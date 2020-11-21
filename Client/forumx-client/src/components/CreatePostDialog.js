import React, { useState, useEffect } from 'react';
import Button from '@material-ui/core/Button';
import TextField from '@material-ui/core/TextField';
import { Box, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@material-ui/core';
import MenuItem from '@material-ui/core/MenuItem';
import PropTypes from 'prop-types';
import { connect } from "react-redux";
import { getAllTopics } from '../actions/topicActions';
import { createPost } from '../actions/postActions';
import { useGoogleReCaptcha } from 'react-google-recaptcha-v3';

const CreatePostDialog = (props) => {

    const {
        open,
        setOpen,
        setCreated,
    } = props;

    const { executeRecaptcha } = useGoogleReCaptcha();

    const { allTopics } = props;
    const { getAllTopics, createPost } = props;

    const [titleError, setTitleError] = useState("");
    const [contentError, setContentError] = useState("");
    const [titleDisable, setTitleDisable] = useState(true);
    const [contentDisable, setContentDisable] = useState(true);

    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");
    const [topic, setTopic] = useState("");

    const handleClose = () => {
        setOpen(false);
    }

    const handleTopicChange = (evt) => {
        setTopic(evt.target.value);
    }

    useEffect(() => {
        if (allTopics.length === 0 && open) {
          getAllTopics(null);
        }
    }, [allTopics, open]);

    const titleValidation = (e) => {
      const tempSubject = (e && e.target && e.target.value) || e;
      setTitle(tempSubject);
      var len = tempSubject.length;
      if (len > 50) {
        setTitleError("Only 50 characters allowed!");
        setTitleDisable(true);
      } else if ( len === 0) {
        setTitleError("Please fill up the title");
        setTitleDisable(true);
      } else {
        setTitleError("");
        setTitleDisable(false);
      }
    };

    const contentValidation = (e) => {
      const tempMessage = (e && e.target && e.target.value) || e;
      setContent(tempMessage);
      var len = tempMessage.length;
      if (len > 512) {
        setContentError("Only 512 characters allowed!");
        setContentDisable(true);
      } else if (len === 0){
        setContentError("Please fill up the message");
        setContentDisable(true);
      } else {
        setContentError("");
        setContentDisable(false);
      }
    };

    const handleCreatePost = async (evt) => {
        evt.preventDefault();
        const captchaToken = await executeRecaptcha('newPost');
        let newPost = {
            Title: title,
            Topic: topic,
            Content: content,
            Captcha: captchaToken
        }
        await createPost(newPost);
        handleClose();
        setCreated(true);
    }

    return (
      <Dialog
        open={open}
        onClose={handleClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Create Post</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Create a post with a subject and a content. Select a topic that your
            post belogs in.
          </DialogContentText>
          <TextField
            autoFocus
            margin="dense"
            name="subject"
            id="subject"
            label="Subject"
            variant="outlined"
            helperText={titleError ? titleError : null}
            error={titleError ? true : false}
            onChange={(e) => titleValidation(e.target.value)}
            required
            fullWidth
          />
          <TextField
            margin="dense"
            name="message"
            id="message"
            label="Message"
            variant="outlined"
            helperText={contentError ? contentError : null}
            error={contentError ? true : false}
            onChange={(e) => contentValidation(e.target.value)}
            required
            fullWidth
            multiline
            rows={8}
          />
          <Box pt={1}>
            <TextField
              id="topic"
              select
              label="Topic"
              variant="outlined"
              value={topic}
              onChange={handleTopicChange}
              helperText="Please select your topic"
              required
            >
              {Object.keys(allTopics).map((key, index) => (
                <MenuItem key={allTopics[key].uuid} value={allTopics[key].uuid}>{allTopics[key].name}</MenuItem>
              ))}
            </TextField>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClose} color="primary">
            Cancel
          </Button>
          <Button
            onClick={handleCreatePost}
            color="primary"
            disabled={contentDisable || titleDisable || topic.length === 0}
          >
            Submit
          </Button>
        </DialogActions>
      </Dialog>
    );
}

const mapStateToProps = (state) => {
  return {
    allTopics: state.forumTopics.topics,
  }
};

const mapDispatchToProps = (dispatch) => {
  return {
      getAllTopics: () => dispatch(getAllTopics()),
      createPost: (post) => dispatch(createPost(post)),
  }
};

CreatePostDialog.propTypes = {
    open: PropTypes.bool,
    setOpen: PropTypes.func,
    setCreated: PropTypes.func,
};

export default connect(mapStateToProps, mapDispatchToProps)(CreatePostDialog);