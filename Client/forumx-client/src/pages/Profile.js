import React, { useMemo, useState, useEffect } from "react";
import { clearLoginError, editPassword, changePasswordCheck } from "../actions/userActions";
import { connect } from "react-redux";
import {
  ThemeProvider,
  makeStyles,
  createMuiTheme,
  Container,
  useMediaQuery,
  CssBaseline,
  Avatar,
  Typography,
  Box,
  TextField,
  Button,
} from "@material-ui/core";
import PersonIcon from "@material-ui/icons/Person";
import ErrorDialog from "../components/ErrorDialog";
import { SuccessBar } from "../components/SuccessBar";
import { clearSuccessMessage } from "../actions/successActions";
import DrawerBar from "../components/NavDrawer";
import { useGoogleReCaptcha } from 'react-google-recaptcha-v3';


const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  root: {
    display: "flex",
  },
  drawer: {
    [theme.breakpoints.up("sm")]: {
      width: drawerWidth,
      flexShrink: 0,
    },
  },
  appBar: {
    [theme.breakpoints.up("sm")]: {
      width: `calc(100% - ${drawerWidth}px)`,
      marginLeft: drawerWidth,
    },
  },
  menuButton: {
    marginRight: theme.spacing(2),
    [theme.breakpoints.up("sm")]: {
      display: "none",
    },
  },
  // necessary for content to be below app bar
  toolbar: theme.mixins.toolbar,
  drawerPaper: {
    width: drawerWidth,
  },
  // content: {
  //   flexGrow: 1,
  //   padding: theme.spacing(3),
  // },
  paper: {
    marginTop: theme.spacing(8),
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    [theme.breakpoints.up("lg")]: {
      marginRight: theme.spacing(25),
    },
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
  bottomPush: {
    position: "fixed",
    bottom: 0,
    textAlign: "center",
    paddingBottom: 10,
    paddingLeft: 5,
  },
  logTable: {
    minWidth: 1000,
  },
  logView: {
    marginTop: theme.spacing(8),
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    [theme.breakpoints.up("lg")]: {
      marginRight: theme.spacing(50),
    },
    [theme.breakpoints.up("sm")]: {
      marginRight: theme.spacing(25),
    },
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
    transition: theme.transitions.create("margin", {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    marginLeft: -drawerWidth,
  },
}));

const Profile = (props) => {
  const [open, setOpen] = useState(false);


  const classes = useStyles();
  const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");
  const { errorMessage, isSuccessful, successMessage, username  , isPasswordChecked, isPasswordCheckedException } = props;
  const { clearLoginError, editPassword, clearSuccessMessage, changePasswordCheck } = props;
  const [password, setPassword] = useState('');
  const [newpassword, setNewPassword] = useState('');
  const [newPasswordError, setNewPasswordError] = useState('');
  const [disabledEdit, setDisableEdit] = useState(false);
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

  const newPasswordValidation = (e) => {
      const tempNewPassword = (e && e.target && e.target.value) || e;
      setNewPassword(tempNewPassword);
      if (tempNewPassword.length < 8 || tempNewPassword.length > 64) {
        setNewPasswordError('Please make sure that password contains more than 8 and less than 64 characters');
        setDisableEdit(true);
      } else if (tempNewPassword === password) {
          setNewPasswordError("New password cannot be the same as password");
          setDisableEdit(true);
      } else {
        setNewPasswordError("");
        setDisableEdit(false);
      }
  }

  const handleCheckPassword = async (evt) => {
      evt.preventDefault();
      const captchaToken = await executeRecaptcha('checkPassword');
      if (newpassword.length >= 8 && newpassword.length < 64) {
        let passwordInfo = {
          captcha: captchaToken,
          password: newpassword,
        }
        changePasswordCheck(passwordInfo);
      }
  }

  const handleEditPassword = (evt) => {
    evt.preventDefault();

    if (password.length > 0 && newpassword.length >= 8) {
      let passwordInfo = {
        OldPassword: password,
        NewPassword: newpassword,
      };
      editPassword(passwordInfo);
    }
  };

  useEffect(() => {
    if (errorMessage.length > 0) {
      setOpen(true);
    }
  }, [errorMessage]);

  return (
    <ThemeProvider theme={theme}>
      <div className={classes.root}>
        <CssBaseline />
        <SuccessBar
          open={isSuccessful}
          setClose={clearSuccessMessage}
          message={successMessage}
        />
        <ErrorDialog
          open={open}
          setOpen={setOpen}
          errorMessage={errorMessage}
          clearError={clearLoginError}
        />
        <DrawerBar username={username} />

        <main className={classes.content}>
          <div className={classes.toolbar} />
          <Container component="main" maxWidth="lg">
            <Box>
              <div className={classes.paper}>
                <Avatar className={classes.avatar}>
                  <PersonIcon />
                </Avatar>
                <Typography component="h1" variant="h5">
                  Change Password
                </Typography>
                <form
                  className={classes.form}
                  noValidate
                  onSubmit={handleEditPassword}
                >
                  <TextField
                    variant="outlined"
                    required
                    fullWidth
                    name="password"
                    label="Current Password"
                    type="password"
                    id="password"
                    onChange={(e) => setPassword(e.target.value)}
                  />
                  <TextField
                    variant="outlined"
                    margin="normal"
                    required
                    fullWidth
                    name="newPassword"
                    label="New Password"
                    type="password"
                    id="newPassword"
                    helperText={newPasswordError}
                    error={newPasswordError}
                    onChange={newPasswordValidation}
                  />

                  <Button
                    fullWidth
                    variant="contained"
                    color="primary"
                    disabled={disabledEdit || newpassword.length === 0 || isPasswordCheckedException}
                    className={classes.submit}
                    onClick={handleCheckPassword}
                    required
                  >
                    Check Password
                  </Button>

                  <Button
                    type="submit"
                    fullWidth
                    variant="contained"
                    color="primary"
                    disabled={!isPasswordChecked || disabledEdit}
                    className={classes.submit}
                    required
                  >
                    Update Password
                  </Button>
                </form>
              </div>
            </Box>
          </Container>
        </main>
      </div>
    </ThemeProvider>
  );
};

const mapStateToProps = (state) => {
  return {
    username: state.user.userName,
    errorMessage: state.user.errorMessage,
    isSuccessful: state.successMessage.isSuccessful,
    successMessage: state.successMessage.message,
    // isPasswordCheckRequesting: state.user.isPasswordCheckRequesting,
    isPasswordChecked: state.user.isPasswordChecked,
    isPasswordCheckedException: state.user.isPasswordCheckedException
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    clearLoginError: () => dispatch(clearLoginError()),
    editPassword: (passwordInfo) => dispatch(editPassword(passwordInfo)),
    clearSuccessMessage: () => dispatch(clearSuccessMessage()),
    changePasswordCheck : (password) => dispatch(changePasswordCheck(password)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Profile);
