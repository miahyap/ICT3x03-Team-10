import React from "react";
import Paper from "@material-ui/core/Paper";
import Typography from "@material-ui/core/Typography";
import { makeStyles } from "@material-ui/core/styles";
import Grid from "@material-ui/core/Grid";
import Box from "@material-ui/core/Box";
import Container from "@material-ui/core/Container";

const useStyles = makeStyles((theme) => ({
  root: {
    color: "#11A873",
  },
  mainFeaturedPost: {
    position: "relative",
    backgroundColor: theme.palette.grey[800],
    color: theme.palette.common.white,
    marginBottom: theme.spacing(4),
    backgroundImage: "url(/images/mountain.jpg)",
    backgroundSize: "cover",
    backgroundRepeat: "no-repeat",
    backgroundPosition: "center",
  },
  overlay: {
    position: "absolute",
    top: 0,
    bottom: 0,
    right: 0,
    left: 0,
    backgroundColor: "rgba(0,0,0,.3)",
  },
  mainFeaturedPostContent: {
    position: "relative",
    padding: theme.spacing(3),
    [theme.breakpoints.up("md")]: {
      padding: theme.spacing(6),
      paddingRight: 0,
    },
  },
}));

const LandingPage = (props) => {
  const { username } = props;
  const classes = useStyles();

  return (
    <Box pt={8} className={classes.paperContainer}>
      <Paper className={classes.mainFeaturedPost}>
        {<img style={{ display: "none" }}  alt="landing-page-view"/>}
        <div className={classes.overlay} />
        <Container maxWidth="lg">
          <Grid container>
            <Grid item md={6}>
              <div className={classes.mainFeaturedPostContent}>
                <Typography variant="h1" gutterBottom>
                  Hi {username},
                </Typography>
                <Typography variant="h3">Welcome to SIT Forum!</Typography>
                <Box>
                  <Typography variant="h5" component="h6">
                    To get started, please select your favourite topic below.
                  </Typography>
                </Box>
              </div>
            </Grid>
          </Grid>
        </Container>
      </Paper>
    </Box>
  );
};

export default LandingPage;
