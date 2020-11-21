import React, { useState, useEffect, useMemo } from "react";
import { connect } from "react-redux";
import { useHistory } from "react-router-dom";
import clsx from "clsx";
import { signUp, generateOTP, verifyOauth, clearRegisterError, checkUsername, checkPassword } from '../actions/userActions';
// UI shitz
import { ThemeProvider, createMuiTheme } from "@material-ui/core/styles";
import {
    Avatar,
    Button,
    CssBaseline,
    TextField,
    Box,
    Typography,
    Grow,
    Grid,
    useMediaQuery,
    CircularProgress,
    Link
} from '@material-ui/core';
import { green } from "@material-ui/core/colors"
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import { makeStyles } from '@material-ui/core/styles';
import ErrorDialog from '../components/ErrorDialog';
import { validateAzureADAccount } from '../helpers/OauthValidation';
import Paper from "@material-ui/core/Paper";
import VerifiedUserIcon from "@material-ui/icons/VerifiedUser";
import QRCode from 'qrcode.react';
import { useGoogleReCaptcha } from 'react-google-recaptcha-v3';

const useStyles = makeStyles((theme) => ({
  root: {
    height: "100vh",
  },
  image: {
    backgroundImage:
      "url(/images/students.jpg)",
    backgroundRepeat: "no-repeat",
    backgroundColor:
      theme.palette.type === "light"
        ? theme.palette.grey[50]
        : theme.palette.grey[900],
    backgroundSize: "cover",
    backgroundPosition: "center",
  },
  paper: {
    margin: theme.spacing(8, 4),
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
  },
  avatar: {
    margin: theme.spacing(1),
    backgroundColor: theme.palette.secondary.main,
  },
  iconText: {
    display: "flex",
    flexDirection: "row",
  },
  icon: {
    marginRight: "10px",
  },
  form: {
    width: "100%", // Fix IE 11 issue.
    marginTop: theme.spacing(1),
  },
  submit: {
    margin: theme.spacing(3, 0, 1),
  },
  alreadyHaveAccount: {
    margin: theme.spacing(3, 0, 2),
  },
  buttonSuccess: {
    backgroundColor: green[500],
    "&:hover": {
      backgroundColor: green[700],
    },
  },
  buttonProgress: {
    color: green[500],
    position: "absolute",
    top: "50%",
    left: "50%",
    marginTop: -12,
    marginLeft: -12,
  },
  wrapper: {
    position: "relative",
  },
  downloadText: {
    marginBottom: "10px",
  },
  mobileText: {
    marginTop: "10px",
    textAlign: "center",
  },
}));
const Register = (props) => {
  let history = useHistory();
  const classes = useStyles();
  const {
    isOTPGenerated,
    fullName,
    isOauthSuccess,
    sitEmail,
    errorMessage,
    usernameValid,
    passwordValid,
    requestingUsername,
    requestingPassword,
    usernameRequestLimit,
    passwordRequestLimit
  } = props;
  const {
    register,
    getOTP,
    verifyOauth,
    clearRegisterError,
    checkUsername,
    checkPassword,
  } = props;

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [open, setOpen] = useState(false);
  const [success, setSuccess] = useState(false);
  const [pwsuccess, setPwSuccess] = useState(false);
  const [usernameError, setUsernameError] = useState("");
  const [usernameAvailability, setUsernameAvailability] = useState(false);
  const [passwordAvailability, setPasswordAvaiilability] = useState(false);
  const [passwordError, setPasswordError] = useState("");
  const [confirmPasswordError, setConfirmPasswordError] = useState("");
  const [registerOtp, setRegisterOtp] = useState('');

  const { executeRecaptcha } = useGoogleReCaptcha();

  const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");
  const theme = useMemo(
    () =>
      createMuiTheme({
        palette: {
          type: prefersDarkMode ? "dark" : "light",
        },
      }),
    [prefersDarkMode]
  );

  const buttonClassname = clsx({
    [classes.buttonSuccess]: success,
  });

  const buttonClasspassword = clsx({
    [classes.buttonSuccess]: pwsuccess,
  });

  const usernameValidation = (e) => {
    const tempUsername = (e && e.target && e.target.value) || e;
    setUsername(tempUsername);
    const re = /^[0-9a-zA-Z_]+$/;
    const isUsername = re.test(String(tempUsername).toLowerCase());
    if (!isUsername) {
      setUsernameError("Username can only contain numbers and alphabet!");
      setUsernameAvailability(false);
    } else if (usernameRequestLimit) {
        setUsernameError('Too much requests. Please refresh the page');
    } else {
      setUsernameError("");
      setUsernameAvailability(true);
    }
  };

  const passwordValidation = (e) => {
    const tempPassword = (e && e.target && e.target.value) || e;
    setPassword(tempPassword);
    if (tempPassword.length < 8) {
      setPasswordError("Password must contain at least 8 characters");
      setPasswordAvaiilability(false);
    } else if (passwordRequestLimit) {
        setPasswordError('Too much requests. Please refresh the page');
        setPasswordAvaiilability(false);
    } else {
      setPasswordError("");
    }
  };

  const confirmPasswordValidation = (e) => {
    const tempConfirmPassword = (e && e.target && e.target.value) || e;
    setConfirmPassword(tempConfirmPassword);
    if (tempConfirmPassword !== password) {
      setConfirmPasswordError("Password and Confirm Password does not match!");
      setPasswordAvaiilability(false);
    } else {
      setConfirmPasswordError("");
      setPasswordAvaiilability(true);
    }
  };

  useEffect(() => {

    if (!isOauthSuccess){
      let params = new URLSearchParams(window.location.search);
      if (params.get("code") != null) {
        let code = params.get("code");
        verifyOauth({ Token: code });
      }
    }else {
      setName(fullName);
      setEmail(sitEmail);
    }
    if (errorMessage.length > 0) {
      setOpen(true);

      if (!isOauthSuccess) {
        history.push('/register');
      }

    }
    if (usernameValid) {
      setSuccess(true);
    }

    if (passwordValid) {
      setPwSuccess(true);
    }
  }, [isOauthSuccess, errorMessage, usernameValid, passwordValid]);

  const handleOTP = (evt) => {
    evt.preventDefault();
    let token = localStorage.getItem("registerCode");
    getOTP({ Token: token }).then(() => {
      let registerOtp = localStorage.getItem("registerOtp");
      setRegisterOtp(registerOtp);
    });
  };

  const handleUsernameAvailbility = async () => {

    const captchaToken = await executeRecaptcha('checkUsername');
    let userInfo = {
      Token: localStorage.getItem("registerCode"),
      Data: username,
      Captcha: captchaToken
    };
    if (usernameRequestLimit) {
        setUsernameAvailability(false);
        history.push('/register');
    } else {
      await checkUsername(userInfo);
    }

  };

  const handlePasswordAvailability = async () => {

    const captchaToken = await executeRecaptcha('checkPassword');
    let userInfo = {
      Token: localStorage.getItem("registerCode"),
      Data: password,
      Captcha: captchaToken
    };
    if (passwordRequestLimit) {
      setPasswordAvaiilability(false);
      history.push('/register');
    } else {
      await checkPassword(userInfo);
    }
  };

  const handleSubmit = async (evt) => {
    evt.preventDefault();
    const captchaToken = await executeRecaptcha('register');
    let token = localStorage.getItem("registerCode");
    if(username && password && token && captchaToken) {
      let data = {
        token: token,
        username: username,
        password: password,
        Captcha: captchaToken,
      };
      await register(data);
    }
  };

  return (
    <ThemeProvider theme={theme}>
      <ErrorDialog
        open={open}
        setOpen={setOpen}
        errorMessage={errorMessage}
        clearError={clearRegisterError}
      />
      <Grid container component="main" className={classes.root}>
        <CssBaseline />
        <Grid item xs={false} sm={4} md={7} className={classes.image} />
        <Grid item xs={12} sm={8} md={5} component={Paper} elevation={6} square>
          <div className={classes.paper}>
            <Avatar className={classes.avatar}>
              <LockOutlinedIcon />
            </Avatar>
            <Typography component="h1" variant="h5">
              Register
            </Typography>
            <form className={classes.form} onSubmit={handleSubmit}>
              <Grid container spacing={2}>
                <Grid item xs={12}>
                  <Button
                    fullWidth
                    variant="contained"
                    data-testid="validateAccount"
                    color={isOauthSuccess ? "default" : "secondary"}
                    style={isOauthSuccess ? { backgroundColor: "green" } : null}
                    className={classes.submit}
                    onClick={isOauthSuccess ? null : validateAzureADAccount}
                    disabled={isOauthSuccess}
                  >
                    {isOauthSuccess ? (
                      <div className={classes.iconText}>
                        <VerifiedUserIcon className={classes.icon} /> SIT
                        Account Validated
                      </div>
                    ) : (
                      "Validate SIT Student Account"
                    )}
                  </Button>
                  <Typography>
                      To get started creating an account, please validate your SIT Student Account.
                  </Typography>
                </Grid>

                <Grid item xs={12}>
                  <TextField
                    variant="outlined"
                    required
                    fullWidth
                    id="fullname"
                    label="Full Name"
                    name="fullname"
                    value={name}
                    data-testid="fullName"
                    onChange={(e) => setName(e.target.value)}
                    disabled
                  />
                </Grid>

                <Grid item xs={12}>
                  <TextField
                    variant="outlined"
                    required
                    fullWidth
                    id="email"
                    label="Email Address"
                    name="email"
                    data-testid="email"
                    autoComplete="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.email)}
                    disabled
                  />
                </Grid>

                {isOauthSuccess ? (
                  <Grow in={isOauthSuccess}>
                    <Grid item xs={12} container>
                      <Grid item xs={8}>
                        <TextField
                          variant="outlined"
                          required
                          fullWidth
                          id="username"
                          label="Username"
                          name="username"
                          error={usernameError}
                          helperText={usernameError}
                          value={username}
                          onChange={(e) => usernameValidation(e.target.value)}
                          data-testid="username"
                        />
                      </Grid>

                      <Grid item xs={4}>
                        <Box mx={2} className={classes.wrapper}>
                          <Button
                            variant="contained"
                            color="secondary"
                            className={buttonClassname}
                            disabled={!usernameAvailability}
                            onClick={handleUsernameAvailbility}
                            data-testid="checkUsernameButton"
                          >
                            Check username
                          </Button>
                          {requestingUsername && (
                            <CircularProgress
                              size={24}
                              className={classes.buttonProgress}
                            />
                          )}
                        </Box>
                      </Grid>
                    </Grid>
                  </Grow>
                ) : null}

                {isOauthSuccess ? (
                  <Grow in={isOauthSuccess}>
                    <Grid item xs={12} container>
                      <Grid item xs={8}>
                        <TextField
                          variant="outlined"
                          required
                          fullWidth
                          name="password"
                          label="Password"
                          type="password"
                          id="password"
                          error={passwordError}
                          helperText={passwordError}
                          onChange={(e) => passwordValidation(e.target.value)}
                          data-testid="password"
                        />
                      </Grid>

                      <Grid item xs={4}>
                        <Box mx={2} className={classes.wrapper}>
                          <Button
                            variant="contained"
                            color="secondary"
                            className={buttonClasspassword}
                            disabled={!passwordAvailability}
                            onClick={handlePasswordAvailability}
                            data-testid="checkPasswordButton"
                          >
                            Check Password
                          </Button>
                          {requestingPassword && (
                            <CircularProgress
                              size={24}
                              className={classes.buttonProgress}
                            />
                          )}
                        </Box>
                      </Grid>
                    </Grid>
                  </Grow>
                ) : null}
                {isOauthSuccess ? (
                  <Grow in={isOauthSuccess}>
                    <Grid item xs={12}>
                      <TextField
                        variant="outlined"
                        required
                        fullWidth
                        name="confirmPassword"
                        label="Confirm Password"
                        type="Password"
                        id="confirmPassword"
                        helperText={confirmPasswordError}
                        error={confirmPasswordError}
                        onChange={(e) =>
                          confirmPasswordValidation(e.target.value)
                        }
                        data-testid="confirmPassword"
                      />
                    </Grid>
                  </Grow>
                ) : null}
              </Grid>

              { isOauthSuccess ?
              (<Typography>
                    To move on, please ensure your username and password is available and valid
              </Typography>) : null}


              {isOauthSuccess ? (
                  <Grow in={isOauthSuccess}>
                    <Button
                      fullWidth
                      variant="contained"
                      color="secondary"
                      className={classes.submit}
                      disabled={!usernameValid || !passwordValid || isOTPGenerated}
                      onClick={handleOTP}
                      data-testid="generateOtpButton"
                    >
                      Generate OTP (OTP is required for login)
                    </Button>
                  </Grow>
                ) : null}

                {isOauthSuccess ? (
                  <Typography className={classes.downloadText}>
                        To access the QR code, download Google Authenticator from Play/App store.
                  </Typography>
                ) : null }

              {isOTPGenerated && registerOtp.length > 0 ? (
                <Grow in={isOTPGenerated && registerOtp} >
                    <Grid container justify="center">
                      <QRCode value={registerOtp} />
                    </Grid>
                </Grow>
              ) : null }

              {isOTPGenerated && registerOtp.length > 0 ? (
                  <Typography className={classes.mobileText}>
                        Click <Link href={registerOtp}>here</Link> to open for mobile
                  </Typography>
              ) : null }

              {isOTPGenerated ? (
                <Grow in={isOTPGenerated}>
                  <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    color="primary"
                    disabled={
                      !usernameValid ||
                      !passwordValid ||
                      confirmPassword !== password
                    }
                    className={classes.submit}
                    data-testid="registerButton"
                  >
                    Sign Up
                  </Button>
                </Grow>
              ) : null}

              <Grid
                container
                justify="flex-end"
                className={isOauthSuccess ? null : classes.alreadyHaveAccount}
              >
                <Grid item>
                  <Link href="/login" variant="body2">
                    Already have an account? Sign in
                  </Link>
                </Grid>
              </Grid>
            </form>
          </div>
        </Grid>
      </Grid>
    </ThemeProvider>
  );
};

const mapStateToProps = (state) => {
  return {
    isOTPGenerated: state.register.otpGenerated,
    fullName: state.register.name,
    isOauthSuccess: state.register.isOauthSuccess,
    sitEmail: state.register.email,
    errorMessage: state.register.errorMessage,
    requestingUsername: state.register.requestingUsername,
    requestingPassword: state.register.requestingPassword,
    usernameValid: state.register.usernameValid,
    passwordValid: state.register.passwordValid,
    usernameRequestLimit: state.register.usernameRequestLimit,
    passwordRequestLimit: state.register.passwordRequestLimit,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    register: (userInfo) => dispatch(signUp(userInfo)),
    getOTP: (token) => dispatch(generateOTP(token)),
    verifyOauth: (token) => dispatch(verifyOauth(token)),
    clearRegisterError: () => dispatch(clearRegisterError()),
    checkUsername: (userInfo) => dispatch(checkUsername(userInfo)),
    checkPassword: (userInfo) => dispatch(checkPassword(userInfo)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Register);