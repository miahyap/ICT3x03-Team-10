import React, { useState } from "react";
import Grid from "@material-ui/core/Grid";
import PersonIcon from "@material-ui/icons/Person";
import Typography from "@material-ui/core/Typography";
import IconButton from "@material-ui/core/IconButton";
import EditIcon from "@material-ui/icons/Edit";
import DeleteIcon from "@material-ui/icons/Delete";
import CheckCircleIcon from "@material-ui/icons/CheckCircle";
import Box from "@material-ui/core/Box";
import Divider from "@material-ui/core/Divider";
import TextField from "@material-ui/core/TextField";
import { makeStyles } from "@material-ui/core";

const useStyles = makeStyles({
  editedText: {
    fontStyle: "italic",
  },
});

export const CommentRow = (props) => {
  const classes = useStyles();

  const { comment, isCommentUser, updateComment, deleteComment } = props;
  const [editable, setEditable] = useState(false);

  const handleDeleteComment = () => {
    deleteComment(comment.uuid);
  };

  const handleEditComment = () => {
    setEditable(true);
    if (editable) {
      let editedComment = document.getElementById("editable-comment").value;
      comment.content = editedComment;
      updateComment(comment);
      setEditable(false);
    }
  };

  return (
    <div>
      <Box pb={2}>
        {editable ? (
          <TextField
            label="Edit comments"
            size="medium"
            id="editable-comment"
            defaultValue={comment.content}
            fullWidth
          />
        ) : (
          <Typography>{comment.content}</Typography>
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
              <Typography>{comment.user}</Typography>
            </Grid>
            <Grid item>
              {comment.edited ? (
                <Box pl={1} className={classes.editedText}>
                  <Typography variant="subtitle1" color="textSecondary">
                    (edited)
                  </Typography>
                </Box>
              ) : null}
            </Grid>
          </Grid>
        </Grid>
        <Grid item xs={1} md={1} justify="flex-end">
          {isCommentUser ? (
            <IconButton onClick={handleEditComment}>
              {editable ? <CheckCircleIcon /> : <EditIcon />}
            </IconButton>
          ) : null}
        </Grid>
        <Grid item xs={1} md={1} justify="flex-end">
          {isCommentUser ? (
            <IconButton onClick={handleDeleteComment}>
              <DeleteIcon />
            </IconButton>
          ) : null}
        </Grid>
      </Grid>
      <Box pb={2}>
        <Divider />
      </Box>
    </div>
  );
};
