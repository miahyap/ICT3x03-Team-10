import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import Toolbar from '@material-ui/core/Toolbar';
import Link from '@material-ui/core/Link';


const useStyles = makeStyles((theme) => ({
    toolbar: {
      borderBottom: `1px solid ${theme.palette.divider}`,
    },
    toolbarTitle: {
      flex: 1,
    },
    toolbarSecondary: {
      justifyContent: 'space-between',
      overflowX: 'auto',
    },
    toolbarLink: {
      padding: theme.spacing(1),
      flexShrink: 0,
    },
  }));

const TopicHeader = (props) => {

    const classes = useStyles();
    const { topics } = props;

    return (
        <React.Fragment>
            <Toolbar component="nav" variant="dense" className={classes.toolbar}>
            {topics.map((topic)=> (
                <Link
                color="inherit"
                noWrap
                key={topic.name}
                variant="body2"
                href={`/main/${topic.uuid}`}
                className={classes.toolbarLink}
                >
                {topic.name}
                </Link>
            ))}
            </Toolbar>
        </React.Fragment>
    );
};

TopicHeader.propTypes = {
    topics: PropTypes.array
};

export default TopicHeader;