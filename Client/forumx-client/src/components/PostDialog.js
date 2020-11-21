import React, { useState, useEffect } from "react";
import moment from "moment";
import { connect } from "react-redux";
import {
  createComment,
  updateComment,
  deleteComment,
} from "../actions/commentActions";
import { updatePost, deletePost, getPost } from "../actions/postActions";
import {
  Dialog,
  DialogContent,
  DialogContentText,
  Box,
  Typography,
  Grid,
  IconButton,
  Container,
  TextField,
  makeStyles,
} from "@material-ui/core";
import { CommentRow } from "./CommentRow";
import EditIcon from "@material-ui/icons/Edit";
import SendIcon from "@material-ui/icons/Send";
import CheckCircleIcon from "@material-ui/icons/CheckCircle";
import QueryBuilderIcon from "@material-ui/icons/QueryBuilder";
import DeleteForeverIcon from "@material-ui/icons/DeleteForever";
import PersonIcon from "@material-ui/icons/Person";
import EventIcon from "@material-ui/icons/Event";
import { useGoogleReCaptcha } from "react-google-recaptcha-v3";

const useStyles = makeStyles({
  cardMedia: {
    width: 160,
  },
  postDialog: {
    overflow: "visible",
  },
  editedText: {
    fontStyle: "italic",
  },
});

const PostDialog = (props) => {
  const { postUuid, open, handleClose } = props;
  const {
    currentPost,
    username,
    isPostUpdated,
    isCommentCreated,
    isCommentUpdated,
    isCommentDeleted,
  } = props;
  const {
    getPost,
    createComment,
    updatePost,
    deletePost,
    updateComment,
    deleteComment,
  } = props;
  const [comment, setComment] = useState("");
  const [commentError, setCommentError] = useState("");
  const [validComment, setValidComment] = useState(true);
  const [editable, setEditable] = useState(false);
  const scroll = "paper";
  const classes = useStyles();

  const { executeRecaptcha } = useGoogleReCaptcha();

  const postComments = currentPost.comments;
  const updatedDateTime = moment(Date.parse(currentPost.postedTime))
    .format("DD/MM/YYYY HH:mm")
    .split(" ", 2);
  currentPost.uuid = postUuid;

  useEffect(() => {
    if (open) {
      getPost(postUuid);
    }
  }, [isPostUpdated, isCommentCreated, isCommentUpdated, isCommentDeleted]);

  const commentValidation = (e) => {
    const tempComment = (e && e.target && e.target.value) || e;
    setComment(tempComment);
    var len = tempComment.length;
    if (len > 128) {
      setCommentError("Only 128 characters allowed!");
      setValidComment(true);
    } else if (len === 0) {
      setCommentError("Please add in your comment");
      setValidComment(false);
    } else {
      setValidComment(false);
    }
  };

  const submitComment = async (evt) => {
    evt.preventDefault();
    const captchaToken = await executeRecaptcha("newComment");
    if (comment.length > 0) {
      let newComment = {
        Post: currentPost.uuid,
        Content: comment,
        Captcha: captchaToken,
      };
      await createComment(newComment);
      setComment("");
    }
  };

  const handleEditPost = () => {
    setEditable(true);

    if (editable) {
      let title = document.getElementById("editable-title").value;
      let content = document.getElementById("editable-content").value;
      currentPost.title = title;
      currentPost.content = content;
      updatePost(currentPost);
      setEditable(false);
    }
  };

  const handleDeletePost = (evt) => {
    evt.preventDefault();
    deletePost(currentPost.uuid);
    handleClose();
  };

  return (
    <div>
      <Dialog
        open={open}
        onClose={handleClose}
        aria-labelledby="form-dialog-title"
        fullWidth
      >
        <DialogContent className={classes.postDialog}>
          <Box pb={1}>
            {editable ? (
              <TextField
                variant="outlined"
                label="Title"
                defaultValue={currentPost.title}
                id="editable-title"
                fullWidth
              ></TextField>
            ) : (
              <Typography component="h2" variant="h5">
                {currentPost.title}
              </Typography>
            )}
          </Box>
          <Grid container>
            <Box pr={0.5}>
              <Grid item>
                <QueryBuilderIcon color="disabled" />
              </Grid>
            </Box>
            <Box pr={1}>
              <Grid item>
                <Typography variant="subtitle1" color="textSecondary">
                  {updatedDateTime[1]}
                </Typography>
              </Grid>
            </Box>
            <Box pr={0.5}>
              <Grid item>
                <EventIcon color="disabled" />
              </Grid>
            </Box>
            <Box pr={1}>
              <Grid item>
                <Typography variant="subtitle1" color="textSecondary">
                  {updatedDateTime[0]}
                </Typography>
              </Grid>
            </Box>
            {currentPost.edited ? (
              <Box className={classes.editedText}>
                <DialogContentText variant="subtitle1" color="textSecondary">
                  (edited)
                </DialogContentText>
              </Box>
            ) : null}
          </Grid>
          <Box pt={2}>
            {editable ? (
              <TextField
                margin="dense"
                name="message"
                id="editable-content"
                label="Message"
                variant="outlined"
                required
                fullWidth
                multiline
                rows={6}
                defaultValue={currentPost.content}
              ></TextField>
            ) : (
              <DialogContentText>{currentPost.content}</DialogContentText>
            )}
          </Box>
          <Grid container xs={12} md={12} direction="row" alignItems="center">
            <Grid item xs={10} md={10}>
              <Grid container>
                <Box pr={0.5}>
                  <Grid item>
                    <PersonIcon />
                  </Grid>
                </Box>
                <Grid item>
                  <Typography>{currentPost.user}</Typography>
                </Grid>
              </Grid>
            </Grid>

            <Grid item xs={1} md={1} justify="flex-end">
              {username === currentPost.user ? (
                <IconButton onClick={handleEditPost}>
                  {editable ? <CheckCircleIcon /> : <EditIcon />}
                </IconButton>
              ) : null}
            </Grid>
            <Grid item xs={1} md={1} justify="flex-end">
              {username === currentPost.user ? (
                <IconButton onClick={handleDeletePost}>
                  <DeleteForeverIcon />
                </IconButton>
              ) : null}
            </Grid>
          </Grid>
        </DialogContent>
        <DialogContent dividers={scroll === "paper"}>
          <DialogContentText variant="h6" color="textSecondary">
            {postComments ? "Comments" : "No Comments"}
          </DialogContentText>
          {currentPost.comments
            ? currentPost.comments.map((comment) => (
                <CommentRow
                  key={comment.uuid}
                  isCommentUser={username === comment.user}
                  comment={comment}
                  updateComment={updateComment}
                  deleteComment={deleteComment}
                />
              ))
            : null}
        </DialogContent>

        <Container>
          <Grid container xs={12} md={12}>
            <Grid item xs={11} md={11}>
              <Box pb={2}>
                <TextField
                  name="Add Comment:"
                  id="comment"
                  label="comment"
                  size="medium"
                  fullWidth
                  value={comment}
                  helperText={commentError ? commentError : null}
                  onChange={(e) => commentValidation(e.target.value)}
                />
              </Box>
            </Grid>
            <Grid item xs={1} md={1} justify="flex-end">
              <IconButton onClick={submitComment} disabled={validComment}>
                <SendIcon />
              </IconButton>
            </Grid>
          </Grid>
        </Container>
      </Dialog>
    </div>
  );
};

const mapStateToProps = (state) => {
  return {
    currentPost: state.forumPost.currentPost,
    username: state.user.userName,
    isPostUpdated: state.forumPost.isPostUpdated,
    isCommentCreated: state.forumComment.isCommentCreated,
    isCommentUpdated: state.forumComment.isCommentUpdated,
    isCommentDeleted: state.forumComment.isCommentDeleted,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    updateComment: (comment) => dispatch(updateComment(comment)),
    createComment: (comment) => dispatch(createComment(comment)),
    deleteComment: (commentUuid) => dispatch(deleteComment(commentUuid)),
    getPost: (postUuid) => dispatch(getPost(postUuid)),
    updatePost: (post) => dispatch(updatePost(post)),
    deletePost: (uuid) => dispatch(deletePost(uuid)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(PostDialog);
