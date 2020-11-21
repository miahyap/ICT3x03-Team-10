import React, { useState, useEffect } from 'react';
import { clearSearchResults, searchResults } from "../actions/searchActions";
import PropTypes from "prop-types";
import { connect } from 'react-redux';
import clsx from 'clsx';
import {
    makeStyles,
    useTheme,
    Drawer,
    CssBaseline,
    AppBar,
    Toolbar,
    List,
    Typography,
    Divider,
    IconButton,
    ListItem,
    ListItemIcon,
    ListItemText,
    InputBase,
    fade,
    Button
} from '@material-ui/core';
import MenuIcon from '@material-ui/icons/Menu';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import SearchIcon from "@material-ui/icons/Search";
import { useHistory } from "react-router-dom";
import HomeIcon from '@material-ui/icons/Home';
import HttpIcon from '@material-ui/icons/Http';
import AccountCircleIcon from '@material-ui/icons/AccountCircle';
import { Link } from "react-router-dom";
import { useGoogleReCaptcha } from 'react-google-recaptcha-v3';
import ErrorDialog from './ErrorDialog';
import { clearCurrentPost } from '../actions/postActions';

const drawerWidth = 240;

const useStyles = makeStyles((theme) => ({
  root: {
    display: 'flex',
  },
  appBar: {
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
  },
  appBarShift: {
    width: `calc(100% - ${drawerWidth}px)`,
    marginLeft: drawerWidth,
    transition: theme.transitions.create(['margin', 'width'], {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
  },
  menuButton: {
    marginRight: theme.spacing(2),
  },
  hide: {
    display: 'none',
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  drawerHeader: {
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(0, 1),
    // necessary for content to be below app bar
    ...theme.mixins.toolbar,
    justifyContent: 'flex-end',
  },
  content: {
    flexGrow: 1,
    padding: theme.spacing(3),
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.leavingScreen,
    }),
    marginLeft: -drawerWidth,
  },
  contentShift: {
    transition: theme.transitions.create('margin', {
      easing: theme.transitions.easing.easeOut,
      duration: theme.transitions.duration.enteringScreen,
    }),
    marginLeft: 0,
  },
  inputRoot: {
    color: 'inherit',
  },
  inputInput: {
    padding: theme.spacing(1, 1, 1, 0),
    // vertical padding + font size from searchIcon
    paddingLeft: `calc(1em + ${theme.spacing(4)}px)`,
    transition: theme.transitions.create('width'),
    width: '100%',
    [theme.breakpoints.up('md')]: {
      width: '20ch',
    },
  },
  search: {
    position: 'relative',
    borderRadius: theme.shape.borderRadius,
    backgroundColor: fade(theme.palette.common.white, 0.15),
    '&:hover': {
      backgroundColor: fade(theme.palette.common.white, 0.25),
    },
    marginRight: theme.spacing(2),
    marginLeft: 0,
    width: '100%',
    [theme.breakpoints.up('sm')]: {
      marginLeft: theme.spacing(3),
      width: 'auto',
    },
  },
  searchIcon: {
    padding: theme.spacing(0, 2),
    height: '100%',
    position: 'absolute',
    pointerEvents: 'none',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  logoutButton: {
    marginLeft: "auto",
    marginRight: -12
  },
}));

const DrawerBar = (props) => {

    let history = useHistory();
    const classes = useStyles();
    const theme = useTheme();
    const { message } = props;
    const { searchResult, clearResult, clearCurrentPost } = props;
    const { username } = props;

    const [open, setOpen] = useState(false);
    const [searchValue, setSearchValue] = useState('');
    const [safeSearch, setSafeSearch] = useState(false);
    const [errorOpen, setErrorOpen] = useState(false);

    const { executeRecaptcha } = useGoogleReCaptcha();

    const handleDrawerOpen = () => {
        setOpen(true);
    };

    const handleDrawerClose = () => {
        setOpen(false);
    };

    useEffect(() => {
      if (message.length > 0) {
        setErrorOpen(true);
      }
    }, [message]);

    const handleSearch = async (evt) => {
        evt.preventDefault();
        const captchaToken = await executeRecaptcha('search');
        if (safeSearch){
          await searchResult(searchValue, captchaToken);
        } else {
          clearResult();

        }
    }

    const handleLogout = (evt) => {
        evt.preventDefault();
        localStorage.clear();
        history.push("/login");
    };


    const whitelistValid = (e) => {
        let desired = e.replace(/[^a-zA-Z0-9]/gi, "");
        if (desired.length >= 5) {
          setSearchValue(desired);
          setSafeSearch(true);
        } else if (desired.length === 0){
          clearResult();
          clearCurrentPost();
        } else {
          setSearchValue('');
          setSafeSearch(false);
        }
    };

    return (
        <div className={classes.root}>
            <CssBaseline />
            <ErrorDialog
                open={errorOpen}
                setOpen={setErrorOpen}
                errorMessage={message}
                clearError={clearResult}
              />
            <AppBar
                position="fixed"
                className={clsx(classes.appBar, {
                [classes.appBarShift]: open,
                })}
            >
                <Toolbar>
                    <IconButton
                        color="inherit"
                        aria-label="open drawer"
                        onClick={handleDrawerOpen}
                        edge="start"
                        className={clsx(classes.menuButton, open && classes.hide)}
                    >
                        <MenuIcon />
                    </IconButton>
                    <Typography variant="h6" noWrap>
                        SIT Forum
                    </Typography>

                    {/* Fix to only display search when there is topic id */}
                    { window.location.pathname === "/logs" || window.location.pathname === "/profile"
                        ? null :
                        <div className={classes.search}>
                            <form onSubmit={handleSearch}>
                                <InputBase
                                placeholder="Search…"
                                classes={{
                                    root: classes.inputRoot,
                                    input: classes.inputInput,
                                }}
                                inputProps={{ 'aria-label': 'search' }}
                                onChange={(e) => whitelistValid(e.target.value)}
                                required
                                />
                                <IconButton type="submit" className={classes.iconButton} aria-label="search">
                                    <SearchIcon />
                                </IconButton>
                            </form>
                        </div>
                    }
                    <section className={classes.logoutButton}>
                        <Button variant="text" aria-label="Logout" onClick={handleLogout}>
                            Log out
                        </Button>
                    </section>

                </Toolbar>
            </AppBar>
            {/* Start of Drawer */}
            <Drawer
                className={classes.drawer}
                variant="persistent"
                anchor="left"
                open={open}
                classes={{
                paper: classes.drawerPaper,
                }}
            >
                <div className={classes.drawerHeader}>
                <Typography variant="h6" noWrap>
                        { username }
                </Typography>
                <IconButton onClick={handleDrawerClose}>
                    {theme.direction === 'ltr' ? <ChevronLeftIcon /> : <ChevronRightIcon />}
                </IconButton>
                </div>
                <Divider />
                <List>
                    <ListItem button component={Link} to="/main">
                         <ListItemIcon>
                                <HomeIcon />
                         </ListItemIcon>
                        <ListItemText primary="Home" />
                    </ListItem>

                    <ListItem button component={Link} to="/profile">
                         <ListItemIcon>
                                <AccountCircleIcon />
                         </ListItemIcon>
                        <ListItemText primary="Profile" />
                    </ListItem>

                    <ListItem button component={Link} to="/logs">
                         <ListItemIcon>
                                <HttpIcon />
                         </ListItemIcon>
                        <ListItemText primary="Logs" />
                    </ListItem>
                </List>
                <Divider />
            </Drawer>
        </div>
    )
}

const mapStateToProps = state => {
    return {
        message: state.search.message,
    }
}

const mapDispatchToProps = (dispatch) => {
  return {
    searchResult: (keyword, captcha) => dispatch(searchResults(keyword, captcha)),
    clearResult: () => dispatch(clearSearchResults()),
    clearCurrentPost: () => dispatch(clearCurrentPost()),
  }
}

DrawerBar.propTypes = {
    username: PropTypes.string
}

export default connect(mapStateToProps, mapDispatchToProps)(DrawerBar);