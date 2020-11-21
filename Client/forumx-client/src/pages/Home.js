import React, { useState, useEffect, useMemo } from "react";
import { useParams } from "react-router";
import { connect } from "react-redux";
import Fab from "@material-ui/core/Fab";
import { makeStyles } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import Container from '@material-ui/core/Container';
import { getAllTopics, getTopicById, loadSearchResults } from '../actions/topicActions';
import TopicHeader from '../components/TopicHeader';
import { clearPostError, resetPostState } from '../actions/postActions';
import { clearCommentError, resetCommentState } from '../actions/commentActions';
import CreatePostDialog from '../components/CreatePostDialog';
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import useMediaQuery from "@material-ui/core/useMediaQuery";
import Post from "../components/Post";
import EditIcon from "@material-ui/icons/Edit";
import CircularProgress from "@material-ui/core/CircularProgress";
import LandingPage from "../components/LandingPage";
import Alert from "@material-ui/lab/Alert";
import { clearSuccessMessage } from "../actions/successActions";
import { SuccessBar } from "../components/SuccessBar";
import DrawerBar from "../components/NavDrawer";

const useStyles = makeStyles((theme) => ({
  root: {
    maxWidth: 345,
    display: "flex",
    "& > * + *": {
      marginLeft: theme.spacing(2),
    },
  },
  fab: {
    margin: 0,
    top: "auto",
    right: 100,
    bottom: 20,
    left: "auto",
    position: "fixed",
    zIndex: 1,
  },
}));

const Home = (props) => {
  const classes = useStyles();
  const {
    allTopics,
    currentTopic,
    username,
    postErrorMessage,
    commentErrorMessage,
    isSuccessful,
    successMessage,
    searchedResult,
    isSearch,
    isPostCreated,
    isPostUpdated,
    isPostDeleted,
  } = props;

  const { getAllTopics,
    getTopic,
    clearPostError,
    clearCommentError,
    clearSuccessMessage,
    loadSearchPost,
    resetPostState,
    resetCommentState
  } = props;

  const { id } = useParams();
  const [loading, setLoading] = useState(true);
  const [openCreatePost, setOpenCreatePost] = useState(false);

  const [searching, setSearching] = useState(false);
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

  const clearMessage = () => {
    clearSuccessMessage();
    resetPostState();
    resetCommentState();
  }


  useEffect(() => {
    if (allTopics.length === 0) {
      getAllTopics(null);
    }
    if (allTopics) {
      setLoading(false);
    }

    if(id && !isSearch && Object.keys(currentTopic).length === 0) {
      getTopic(id);
    }

    if (isPostCreated || isPostUpdated || isPostDeleted) {
      if (id) {
        getTopic(id);
      }
    }

    if (searchedResult.length > 0) {
      loadSearchPost(searchedResult);
      setSearching(true);
    }
  }, [isSearch, searchedResult, isPostCreated, isPostUpdated, isPostDeleted]);

  const closeWarningMessage = () => {
    if (postErrorMessage) {
      clearPostError();
    }
    if (commentErrorMessage) {
      clearCommentError();
    }
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <SuccessBar
        open={isSuccessful}
        setClose={clearMessage}
        message={successMessage}
      />
      {loading ? (
        <div className={classes.root}>
          <CircularProgress color="secondary" />
        </div>
      ) : (
        <div>
          <LandingPage username={username} />
          <DrawerBar username={username} />
          <Container maxWidth="lg">
            <CreatePostDialog
              open={openCreatePost}
              setOpen={setOpenCreatePost}
              setCreated={setSearching}
            />

            <main>
              <TopicHeader topics={allTopics} />
              {postErrorMessage || commentErrorMessage ? (
                <Alert
                  variant="outlined"
                  severity="error"
                  onClose={() => closeWarningMessage()}
                >
                  This is something wrong here. Pleae try again later
                </Alert>
              ) : null}

              {searching || currentTopic.posts
                ? currentTopic.posts.map((post) => (
                    <Post post={post} key={post.uuid} />
                  ))
                : null}
              <Fab
                color="primary"
                aria-label="Create"
                className={classes.fab}
                onClick={(e) => setOpenCreatePost(true)}
              >
                <EditIcon />
              </Fab>
            </main>
          </Container>
        </div>
      )}
    </ThemeProvider>
  );
};

const mapStateToProps = (state) => {
  return {
    allTopics: state.forumTopics.topics,
    currentTopic: state.forumTopics.currentTopic,
    username: state.user.userName,
    postErrorMessage: state.forumPost.errorMessage,
    commentErrorMessage: state.forumComment.errorMessage,
    isSuccessful: state.successMessage.isSuccessful,
    successMessage: state.successMessage.message,
    isSearch: state.search.searched,
    searchedResult: state.search.searchedResult,
    isPostCreated: state.forumPost.isPostCreated,
    isPostUpdated: state.forumPost.isPostUpdated,
    isPostDeleted: state.forumPost.isPostDeleted,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
      getAllTopics: () => dispatch(getAllTopics()),
      getTopic: (topicUuid) => dispatch(getTopicById(topicUuid)),
      clearPostError: () => dispatch(clearPostError()),
      clearCommentError : () => dispatch(clearCommentError()),
      clearSuccessMessage: () => dispatch(clearSuccessMessage()),
      loadSearchPost: (searchResults) => dispatch(loadSearchResults(searchResults)),
      resetPostState: () => dispatch(resetPostState()),
      resetCommentState: () => dispatch(resetCommentState()),
  }
};

export default connect(mapStateToProps, mapDispatchToProps)(Home);
