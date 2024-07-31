using System.Data;
using System.Reflection;
using Moq;
using Sprache;
using Xunit;

public class AuhServiceTest
{
    [Fact]
    public async void should_return_error_user_not_registered_when_logging_in()
    {
        var userRepositoyMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var convertUserRegisterDtoMock = new Mock<IConvertUserRegisterDto>();
        var convertTokenMock = new Mock<IConvertToken>();
        var jwtServiceMock = new Mock<IJwtService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();

        var authService = new AuthService(userRepositoyMock.Object, passwordServiceMock.Object, convertUserRegisterDtoMock.Object, jwtServiceMock.Object, unitOfWorkMock.Object, convertTokenMock.Object, tokenRepositoryMock.Object);

        var userLoginDto = new UserLoginDto();

        userRepositoyMock.Setup(u => u.FindUserByEmail(userLoginDto.email)).Returns((UserModel)null);

        var result = await authService.Login(userLoginDto);


        Assert.IsType<UserNotRegisteredError>(result.AsT1);
    }
    [Fact]
    public async void should_return_error_password_incorrect_when_logging_in()
    {
        var userRepositoyMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var convertUserRegisterDtoMock = new Mock<IConvertUserRegisterDto>();
        var convertTokenMock = new Mock<IConvertToken>();
        var jwtServiceMock = new Mock<IJwtService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();

        var authService = new AuthService(userRepositoyMock.Object, passwordServiceMock.Object, convertUserRegisterDtoMock.Object, jwtServiceMock.Object, unitOfWorkMock.Object, convertTokenMock.Object, tokenRepositoryMock.Object);

        var userLoginDto = new UserLoginDto();
        var userModel = new UserModel("erick", userLoginDto.email, userLoginDto.password);
        userRepositoyMock.Setup(u => u.FindUserByEmail(userLoginDto.email)).Returns(userModel);
        passwordServiceMock.Setup(p => p.VerifyPassword(userLoginDto.password, userModel.password)).Returns(false);

        var result = await authService.Login(userLoginDto);
        Assert.IsType<IncorrectPasswordError>(result.AsT1);
    }
    [Fact]
    public async void should_login_in_user()
    {
        var userRepositoyMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var convertUserRegisterDtoMock = new Mock<IConvertUserRegisterDto>();
        var convertTokenMock = new Mock<IConvertToken>();
        var jwtServiceMock = new Mock<IJwtService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();

        var authService = new AuthService(userRepositoyMock.Object, passwordServiceMock.Object, convertUserRegisterDtoMock.Object, jwtServiceMock.Object, unitOfWorkMock.Object, convertTokenMock.Object, tokenRepositoryMock.Object);

        var userLoginDto = new UserLoginDto();
        var userModel = new UserModel("erick", userLoginDto.email, userLoginDto.password);
        var refreshToken = "teste";
        var accesstoken = "teste";
        userRepositoyMock.Setup(u => u.FindUserByEmail(userLoginDto.email)).Returns(userModel);
        passwordServiceMock.Setup(p => p.VerifyPassword(userLoginDto.password, userModel.password)).Returns(true);
        jwtServiceMock.Setup(j => j.GenerateAccessToken(userModel)).Returns(accesstoken);
        jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns(refreshToken);
        tokenRepositoryMock.Setup(t => t.UpdateToken(userModel.email, refreshToken));

        var result = await authService.Login(userLoginDto);

        var response = new ResponseAuth(200, "úsuario logado com sucesso", refreshToken, accesstoken);

        Assert.Equal(response.message, result.AsT0.message);
        Assert.Equal(response.status, result.AsT0.status);
        Assert.Equal(response.refreshToken, refreshToken);
        Assert.Equal(response.accessToken, accesstoken);
    }
    [Fact]
    public async void should_return_error_email_already_registered_when_user_register()
    {
        var userRepositoyMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var convertUserRegisterDtoMock = new Mock<IConvertUserRegisterDto>();
        var convertTokenMock = new Mock<IConvertToken>();
        var jwtServiceMock = new Mock<IJwtService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();

        var authService = new AuthService(userRepositoyMock.Object, passwordServiceMock.Object, convertUserRegisterDtoMock.Object, jwtServiceMock.Object, unitOfWorkMock.Object, convertTokenMock.Object, tokenRepositoryMock.Object);

        var userRegisterDto = new UserRegisterDto("erick", "erickjb93@gmail.com", "sirlei231");
        var userModel = new UserModel("erick", userRegisterDto.email, userRegisterDto.password);
        userRepositoyMock.Setup(u => u.FindUserByEmail(userRegisterDto.email)).Returns(userModel);

        var result = await authService.Register(userRegisterDto);


        Assert.IsType<EmailIsAlreadyRegisteredError>(result.Value);
    }
    [Fact]
    public async void should_register_user()
    {
        var userRepositoyMock = new Mock<IUserRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var convertUserRegisterDtoMock = new Mock<IConvertUserRegisterDto>();
        var convertTokenMock = new Mock<IConvertToken>();
        var jwtServiceMock = new Mock<IJwtService>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var tokenRepositoryMock = new Mock<ITokenRepository>();
        var transactionMock = new Mock<IDbTransaction>();

        var authService = new AuthService(userRepositoyMock.Object, passwordServiceMock.Object, convertUserRegisterDtoMock.Object, jwtServiceMock.Object, unitOfWorkMock.Object, convertTokenMock.Object, tokenRepositoryMock.Object);

        var userRegisterDto = new UserRegisterDto("erick", "erickjb93@gmail.com", "sirlei231");
        var refreshToken = "teste";
        var accesstoken = "teste";
        var encryptPassword = "teste";
        var userModel = new UserModel("erick", userRegisterDto.email, userRegisterDto.password);
        var tokenModel = new TokenModel(userModel.email, refreshToken);

        userRepositoyMock.Setup(u => u.FindUserByEmail(userRegisterDto.email)).Returns((UserModel) null);
        passwordServiceMock.Setup(p => p.EncryptPassword(userRegisterDto.password)).Returns(encryptPassword);
        convertUserRegisterDtoMock.Setup(c => c.convertInUserModel(userRegisterDto)).Returns(userModel);
        jwtServiceMock.Setup(j => j.GenerateAccessToken(userModel)).Returns(accesstoken);
        jwtServiceMock.Setup(j => j.GenerateRefreshToken()).Returns(refreshToken);
        convertTokenMock.Setup(c => c.ConvertInTokenModel(refreshToken, userModel.email)).Returns(tokenModel);
        unitOfWorkMock.Setup(u => u.BeginTransaction()).Returns(transactionMock.Object);
        userRepositoyMock.Setup(u => u.Register(userModel));
        tokenRepositoryMock.Setup(t => t.RegisterToken(tokenModel));

        var response = new ResponseAuth(201, "úsuario cadastrado com sucesso",refreshToken,accesstoken);

        var result = await authService.Register(userRegisterDto);
        Assert.True(result.IsT0);
        Assert.IsType<ResponseAuth>(result.AsT0);

    }
}