using MediatR;
using NextERP.Common.Constants;
using NextERP.Common.DTOs;
using NextERP.Core.Entities;
using NextERP.Core.Enums;
using NextERP.Core.Interfaces;
using NextERP.Features.Auth.DTOs;
using NextERP.Infrastructure.Data;

namespace NextERP.Features.Auth.Commands
{
    public record RegisterCommand(RegisterRequestDto Request) : IRequest<ApiResponse<RegisterResponseDto>>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<RegisterResponseDto>>
    {
        private readonly IRepository<TenantModel> _tenantRepository;
        private readonly IRepository<UserModel> _userRepository;
        private readonly IRepository<RoleModel> _roleRepository;
        private readonly IRepository<UserRoleModel> _userRoleRepository;
        private readonly IRepository<PermissionModel> _permissionRepository;
        private readonly IRepository<RolePermissionModel> _rolePermissionRepository;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public RegisterCommandHandler(
            IRepository<TenantModel> tenantRepository,
            IRepository<UserModel> userRepository,
            IRepository<RoleModel> roleRepository,
            IRepository<UserRoleModel> userRoleRepository,
            IRepository<PermissionModel> permissionRepository,
            IRepository<RolePermissionModel> rolePermissionRepository,
            IJwtService jwtService,
            ApplicationDbContext context)
        {
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _jwtService = jwtService;
            _context = context;
        }

        public async Task<ApiResponse<RegisterResponseDto>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            
            // Start a database transaction
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            
            try
            {
                // 1. Check if subdomain already exists
                var subdomainExists = await _tenantRepository.AnyAsync(t => t.Subdomain == request.Subdomain, cancellationToken);
                if (subdomainExists)
                    return ApiResponse<RegisterResponseDto>.Error("Subdomain already taken", 409);

                // 2. Check if username or email already exists globally
                var userExists = await _userRepository.AnyAsync(u => u.Username == request.Username || u.Email == request.Email, cancellationToken);
                if (userExists)
                    return ApiResponse<RegisterResponseDto>.Error("Username or email already exists", 409);

                // 3. Create Tenant
                var tenant = new TenantModel
                {
                    CompanyName = request.CompanyName,
                    Subdomain = request.Subdomain,
                    TaxId = request.TaxId,
                    Address = request.Address,
                    Phone = request.PhoneNumber,
                    Email = request.Email,
                    Status = ENUM_UserStatus.Active
                };
                tenant = await _tenantRepository.AddAsync(tenant, cancellationToken);

                // 4. Create Admin User
                var adminUser = new UserModel
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    Address = request.Address,
                    TenantId = tenant.TenantId,
                    Status = ENUM_UserStatus.Active
                };
                adminUser = await _userRepository.AddAsync(adminUser, cancellationToken);

                // 5. Create 'Admin' role for this tenant
                var adminRole = new RoleModel
                {
                    RoleName = "Admin",
                    Description = "Company Administrator with full permissions",
                    TenantId = tenant.TenantId,
                    IsSystemRole = true,
                    Status = ENUM_UserStatus.Active
                };
                adminRole = await _roleRepository.AddAsync(adminRole, cancellationToken);

                // 6. Ensure global permissions exist (insert only once, on first registration or if missing)
                var existingPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
                if (!existingPermissions.Any())
                {
                    var allPermissions = SystemPermissions.GetAllPermissions();
                    foreach (var perm in allPermissions)
                    {
                        await _permissionRepository.AddAsync(perm, cancellationToken);
                    }
                    existingPermissions = allPermissions;
                }

                // 7. Assign all permissions to the Admin role
                foreach (var perm in existingPermissions)
                {
                    await _rolePermissionRepository.AddAsync(new RolePermissionModel
                    {
                        RoleId = adminRole.RoleId,
                        PermissionId = perm.PermissionId
                    }, cancellationToken);
                }

                // 8. Assign Admin role to the admin user
                await _userRoleRepository.AddAsync(new UserRoleModel
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.RoleId,
                    AssignedBy = adminUser.UserId
                }, cancellationToken);

                // 9. Generate JWT tokens for the admin user
                // This is the critical point where errors must trigger rollback
                string accessToken;
                string refreshToken;
                DateTime expiry;
                
                try
                {
                    var tokenResult = await _jwtService.GenerateTokensAsync(adminUser, cancellationToken);
                    accessToken = tokenResult.AccessToken;
                    refreshToken = tokenResult.RefreshToken;
                    expiry = tokenResult.Expiry;
                }
                catch (InvalidOperationException ex)
                {
                    // Rollback transaction on JWT generation failure
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponse<RegisterResponseDto>.Error(
                        $"Failed to generate authentication tokens: {ex.Message}. Registration has been rolled back.",
                        500);
                }
                catch (Exception ex)
                {
                    // Rollback transaction on any other error
                    await transaction.RollbackAsync(cancellationToken);
                    return ApiResponse<RegisterResponseDto>.Error(
                        $"An unexpected error occurred during registration: {ex.Message}. Registration has been rolled back.",
                        500);
                }

                // 10. Prepare response
                var response = new RegisterResponseDto
                {
                    UserId = adminUser.UserId,
                    Email = adminUser.Email,
                    FirstName = adminUser.FirstName,
                    LastName = adminUser.LastName,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiresIn = (int)(expiry - DateTime.UtcNow).TotalSeconds,
                    Roles = new List<string> { "Admin" },
                    Permissions = existingPermissions.Select(p => p.PermissionName).ToList()
                };

                // Commit transaction
                await transaction.CommitAsync(cancellationToken);
                
                return ApiResponse<RegisterResponseDto>.Ok(response, "Company and admin account created successfully");
            }
            catch (Exception ex)
            {
                // Rollback transaction on any uncaught exception
                await transaction.RollbackAsync(cancellationToken);
                return ApiResponse<RegisterResponseDto>.Error(
                    $"Registration failed: {ex.Message}. All changes have been rolled back.",
                    500);
            }
        }
    }
}
