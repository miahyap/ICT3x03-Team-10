import React, { useState, useEffect, useMemo } from 'react';
import { connect } from 'react-redux';
import { useHistory } from "react-router-dom";
import { login, verifyOTP, clearLoginError } from '../actions/userActions';
import { ThemeProvider, createMuiTheme } from "@material-ui/core/styles";
import useMediaQuery from "@material-ui/core/useMediaQuery";
import Avatar from '@material-ui/core/Avatar';
import Button from '@material-ui/core/Button';
import CssBaseline from '@material-ui/core/CssBaseline';
import TextField from '@material-ui/core/TextField';
import Grid from '@material-ui/core/Grid';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from '@material-ui/core/Typography';
import { makeStyles } from '@material-ui/core/styles';
import Link from '@material-ui/core/Link';
import Paper from "@material-ui/core/Paper";
import ErrorDialog from '../components/ErrorDialog';
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
    form: {
      width: "100%", // Fix IE 11 issue.
      marginTop: theme.spacing(1),
    },
    submit: {
      margin: theme.spacing(3, 0, 2),
    },
  }));

const Login = (props) => {

    const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");
    let history = useHistory();
    const { isLoggedInSubmitted, errorMessage } = props;
    const { validateOTP, signIn, clearLoginError } = props;

    const classes = useStyles();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [otpDigit, setOtpDigit] = useState("");
    const [open, setOpen] = useState(false);

    const [usernameError, setUsernameError] = useState("");
    const [optError, setOptError] = useState("");

    const { executeRecaptcha } = useGoogleReCaptcha();

    const theme = useMemo(
    () =>
        createMuiTheme({
        palette: {
            type: prefersDarkMode ? "dark" : "light",
        },
        }),
    [prefersDarkMode]
    );

    useEffect(() => {
        if (errorMessage.length > 0){
            setOpen(true);
        }
    }, [errorMessage]);

    const usernameValidation = (e) => {
        const tempUsername = (e && e.target && e.target.value) || e;
        setUsername(tempUsername);
        const re = /^[0-9a-zA-Z_]+$/;
        const isUsername = re.test(String(tempUsername).toLowerCase());
        if (!isUsername) {
          setUsernameError("Invalid Username!");
          setUsername('');
        } else {
          setUsernameError("");
        }
      };

    const otpValidation = (e) => {
        const tempOtp = (e && e.target && e.target.value) || e;
        setOtpDigit(tempOtp);
        const otpLen = tempOtp.length;
        const isValidOtp = (otpLen === 6 && tempOtp !== "000000");
        if (!isValidOtp) {
            setOptError("Invalid OTP");
        } else {
            setOptError("");
        }
    };

    const handleLogin = async (evt) => {
        evt.preventDefault();
        const captchaToken = await executeRecaptcha('login');
        if (username.length > 0 && password.length > 0) {
            let data = {
                Username: username,
                Password: password,
                Captcha: captchaToken
            };
            await signIn(data);
        }
    }

    const handleVerifyOTP = async (evt) => {
        evt.preventDefault();
        let accessToken = localStorage.getItem("AccessToken");
        const captchaToken = await executeRecaptcha('verifyOTP');
        let tokenData = {
            Token: accessToken,
            Data: otpDigit.toString(),
            Captcha: captchaToken
        };
        validateOTP(tokenData).then(() => {
            history.push('/main');
        });
    };

    return (
        <ThemeProvider theme={theme}>
            <ErrorDialog open={open} setOpen={setOpen} errorMessage={errorMessage}  clearError={clearLoginError} />

            <Grid container component="main" className={classes.root}>
                <CssBaseline />
                <Grid item xs={false} sm={4} md={7} className={classes.image} />
                <Grid item xs={12} sm={8} md={5} component={Paper} elevation={6} square>
                <div className={classes.paper}>
                    <Avatar className={classes.avatar}>
                    <LockOutlinedIcon />
                    </Avatar>
                    <Typography component="h1" variant="h5">
                    Sign in
                    </Typography>
                    <form className={classes.form} onSubmit={handleVerifyOTP}>
                    <TextField
                        variant="outlined"
                        margin="normal"
                        required={true}
                        fullWidth
                        id="username"
                        label="Username"
                        name="username"
                        error={usernameError}
                        helperText={usernameError}
                        onChange={usernameValidation}
                        data-testid="username"
                    />

                    <TextField
                        variant="outlined"
                        margin="normal"
                        required={true}
                        fullWidth
                        name="password"
                        label="Password"
                        type="password"
                        id="password"
                        data-testid="password"
                        onChange={(e) => setPassword(e.target.value)}
                    />
                    <Button
                        type="submit"
                        onClick={handleLogin}
                        fullWidth
                        variant="contained"
                        color="primary"
                        data-testid="signInButton"
                        className={classes.submit}
                        disabled={!username || !password}
                    >
                        Sign In
                    </Button>

                    {isLoggedInSubmitted ? (
                        <TextField
                        variant="outlined"
                        margin="normal"
                        required
                        fullWidth
                        id="otpDigit"
                        label="6 Digit OTP"
                        name="otpDigit"
                        type="number"
                        data-testid="otp"
                        error={optError}
                        helperText={optError}
                        disabled={!isLoggedInSubmitted}
                        value={otpDigit}
                        onChange={otpValidation}
                        />
                    ) : null}

                    {isLoggedInSubmitted ? (
                        <Button
                        fullWidth
                        variant="contained"
                        color="secondary"
                        type="submit"
                        data-testid="validateOtpButton"
                        disabled={!isLoggedInSubmitted && otpDigit !== 6}
                        className={classes.submit}
                        >
                        Validate OTP
                        </Button>
                    ) : null}

                    <Grid container>
                        <Grid item xs>
                        <Link href="/resetpassword" variant="body2">
                            Forgot password?
                        </Link>
                        </Grid>
                        <Grid item>
                        <Link href="/register" variant="body2">
                            {"Don't have an account? Sign Up"}
                        </Link>
                        </Grid>
                    </Grid>
                    </form>
                </div>
                </Grid>
            </Grid>
        </ThemeProvider>
    )
};


const mapStateToProps = state => {
    return {
        isLoggedInSubmitted: state.user.submitted,
        errorMessage: state.user.errorMessage,
    }
};

const mapDispatchToProps = (dispatch) => {
    return {
        validateOTP: (tokenData) => dispatch(verifyOTP(tokenData)),
        signIn: (userInfo) => dispatch(login(userInfo)),
        clearLoginError: () => dispatch(clearLoginError()),
    }
};
export default connect(mapStateToProps, mapDispatchToProps)(Login);
