import React, { useState, useEffect, useMemo} from 'react';
import { clearLoginError, resetPassword } from '../actions/userActions';
import { connect } from 'react-redux';
import { ThemeProvider, createMuiTheme } from "@material-ui/core/styles";
import { makeStyles } from '@material-ui/core/styles';
import Container from '@material-ui/core/Container';
import useMediaQuery from "@material-ui/core/useMediaQuery";
import Avatar from '@material-ui/core/Avatar';
import CssBaseline from '@material-ui/core/CssBaseline';
import Grid from '@material-ui/core/Grid';
import ErrorDialog from '../components/ErrorDialog';
import LockOutlinedIcon from '@material-ui/icons/LockOutlined';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import Link from '@material-ui/core/Link';
import { validateAzureADAccount } from '../helpers/OauthValidation';
import { SuccessBar } from '../components/SuccessBar';
import { clearSuccessMessage } from '../actions/successActions';
import { useGoogleReCaptcha } from 'react-google-recaptcha-v3';

const useStyles = makeStyles((theme) => ({
    paper: {
      marginTop: theme.spacing(8),
      display: 'flex',
      flexDirection: 'column',
      alignItems: 'center',
    },
    avatar: {
      margin: theme.spacing(1),
      backgroundColor: theme.palette.secondary.main,
    },
    form: {
      width: '100%', // Fix IE 11 issue.
      marginTop: theme.spacing(3),
    },
    submit: {
      margin: theme.spacing(3, 0, 2),
    },
}));

const ResetPassword = (props) => {

    const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");
    const { errorMessage, isSuccessful, successMessage } = props;
    const { resetPassword, clearLoginError, clearSuccessMessage } = props;

    const classes = useStyles();
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [open, setOpen] = useState(false);
    const [isValidated, setIsValidated] = useState(false);

    const [usernameError, setUsernameError] = useState("Please enter username");
    const [passwordError, setPasswordError] = useState("Please enter password");

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

    const checkIfAuthComplete = () => {
        let params = new URLSearchParams(window.location.search);

        if (params.get("code") != null) {
            let code = params.get("code");
            localStorage.setItem('reset-code', code);
            setIsValidated(true);
        }
    }

    useEffect(() => {
        if (errorMessage.length > 0){
            setOpen(true);
        }
        if (!localStorage.getItem('reset-code')){
            checkIfAuthComplete();
        }
        return () => {
            localStorage.removeItem('reset-code');
        }
    });

    const usernameValidation = (e) => {
        const tempUsername = (e && e.target && e.target.value) || e;
        setUsername(tempUsername);
        const re = /^[0-9a-zA-Z_]+$/;
        const isUsername = re.test(String(tempUsername).toLowerCase());
        if (!isUsername) {
          setUsernameError("Invalid Username!");
        } else {
          setUsernameError("");
        }
    };

    const passwordValidation = (e) => {
        const tempPassword = (e && e.target && e.target.value) || e;
        setPassword(tempPassword);
        if (tempPassword.length < 8 || tempPassword.length > 64) {
            setPasswordError("Please ensure password is more than 8 and less than 64 characters");
        } else {
            setPasswordError("");
        }
    }

    const handleSubmit = async (evt) => {
        evt.preventDefault();
        const captchaToken = await executeRecaptcha('resetpassword');
        const registerToken = localStorage.getItem('reset-code');
        if (registerToken && username && password) {
            const data = {
                token: registerToken,
                username: username,
                password: password,
                captcha: captchaToken,
            };
            await resetPassword(data);
        }
    }

    return (
        <ThemeProvider theme={theme}>
            <Container component="main" maxWidth="xs">
                <CssBaseline />

                <ErrorDialog open={open} setOpen={setOpen} errorMessage={errorMessage}  clearError={clearLoginError} />
                <SuccessBar
                    open={isSuccessful}
                    setClose={clearSuccessMessage}
                    message={successMessage}
                />
                <div className={classes.paper}>
                    <Avatar className={classes.avatar}>
                    <LockOutlinedIcon />
                    </Avatar>
                    <Typography component="h1" variant="h5">
                    Reset Password
                    </Typography>
                    <form className={classes.form} onSubmit={handleSubmit}>

                    <Grid container spacing={2}>
                        <Grid item xs={12}>
                            <Button
                                fullWidth
                                variant="contained"
                                color="secondary"
                                className={classes.submit}
                                onClick={validateAzureADAccount}
                                disabled={isValidated}
                                data-testid="validateAccountButton"
                            >
                                Validate SIT Student Account
                            </Button>
                        </Grid>

                        <Grid item xs={12}>
                            <TextField
                                variant="outlined"
                                required
                                fullWidth
                                id="username"
                                label="Username"
                                name="username"
                                data-testid="username"
                                onChange={usernameValidation}
                                error={usernameError}
                                helperText={usernameError}

                            />
                        </Grid>

                        <Grid item xs={12}>
                            <TextField
                                variant="outlined"
                                required
                                fullWidth
                                name="password"
                                label="New Password"
                                type="password"
                                id="password"
                                data-testid="password"
                                error={passwordError}
                                helperText={passwordError}
                                onChange={passwordValidation}
                            />
                        </Grid>
                    </Grid>

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        color="primary"
                        disabled={usernameError.length !== 0 || passwordError.length !== 0 || !isValidated }
                        className={classes.submit}
                        data-testid="resetPasswordButton"
                    >
                        Reset Password
                    </Button>

                    <Grid container justify="flex-end">
                        <Grid item>
                        <Link href="/login" variant="body2">
                            Already have an account? Sign in
                        </Link>
                        </Grid>
                    </Grid>
                    </form>
                </div>
            </Container>
        </ThemeProvider>
    );
};

const mapStateToProps = state => {
    return {
        errorMessage: state.user.errorMessage,
        isSuccessful: state.successMessage.isSuccessful,
        successMessage: state.successMessage.message,
    }
}

const mapDispatchToProps = (dispatch) => {
    return {
        resetPassword: (userInfo) => dispatch(resetPassword(userInfo)),
        clearLoginError: () => dispatch(clearLoginError()),
        clearSuccessMessage: () => dispatch(clearSuccessMessage()),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(ResetPassword);