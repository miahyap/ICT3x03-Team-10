import React, { useState, useEffect } from "react";
import { connect } from "react-redux";
import moment from 'moment';
import {
  Box,
  Card,
  CardContent,
  Typography,
  CardActionArea,
  Grid,
  makeStyles,
} from "@material-ui/core";
import EventIcon from "@material-ui/icons/Event";
import QueryBuilderIcon from "@material-ui/icons/QueryBuilder";
import PersonIcon from "@material-ui/icons/Person";
import PostDialog from './PostDialog';
import { getPost } from "../actions/postActions";

const useStyles = makeStyles({
  card: {
    display: "flex",
  },
  cardDetails: {
    flex: 1,
  }
});

const Post = (props) => {

    const classes = useStyles();
    const { post } = props;
    const { getPost } = props;
    const [open, setOpen] = useState(false);
    const updatedDateTime = moment(Date.parse(post.postedTime)).format('DD/MM/YYYY HH:mm').split(" ", 2);

    const handleClickOpen = () => {
      setOpen(true);
    };

  const handleClose = () => {
    setOpen(false);
  };

  useEffect(() => {
    if (open) {
      getPost(post.uuid);
    }
  }, [open]);

  return (
    <div>
      <PostDialog
        postUuid={post.uuid}
        open={open}
        handleClose={handleClose}
      />

      <Box pt={2} boxShadow={2}>
        <CardActionArea component="a" onClick={handleClickOpen}>
          <Card className={classes.card}>
            <div className={classes.cardDetails}>
              <CardContent>
                <Box pb={1}>
                  <Typography component="h2" variant="h5">
                    {post.title}
                  </Typography>
                </Box>
                <Grid container>
                  <Box pr={0.5}>
                    <Grid item>
                      <QueryBuilderIcon color="disabled" />
                    </Grid>
                  </Box>
                  <Box pr={0.5}>
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
                  <Box pr={0.5}>
                    <Grid item>
                      <Typography variant="subtitle1" color="textSecondary">
                        {updatedDateTime[0]}
                      </Typography>
                    </Grid>
                  </Box>
                </Grid>
                <Box pt={2}>
                  <Typography variant="subtitle1" paragraph>
                    {post.content}
                  </Typography>
                </Box>
                <Grid container xs={12} md={12}>
                  <Grid item xs={11} md={11}>
                    <Grid container direction="row" alignItems="center">
                      <Grid item>
                        <Box pr={1}>
                          <PersonIcon />
                        </Box>
                      </Grid>
                      <Grid item>
                        <Typography>{post.user}</Typography>
                      </Grid>
                    </Grid>
                  </Grid>
                </Grid>
              </CardContent>
            </div>
          </Card>
        </CardActionArea>
      </Box>
    </div>
  );
};

const mapStateToProps = (state) => {
  return {
    currentPost: state.forumPost.currentPost,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    getPost: (postUuid) => dispatch(getPost(postUuid)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Post);