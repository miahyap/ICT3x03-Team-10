import React, { useState, useEffect, useMemo } from "react";
import { connect } from "react-redux";
import { getActivityLogs, terminateSession } from "../actions/userActions";
import { makeStyles } from "@material-ui/core/styles";
import moment from "moment";
import DrawerBar from "../components/NavDrawer";
import {
  Button,
  Typography,
  CssBaseline,
  useMediaQuery,
  ThemeProvider,
  Paper,
  Table,
  createMuiTheme,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
} from "@material-ui/core";

const columns = [
  { id: "name", label: "Name", minWidth: 170 },
  {
    id: "source",
    label: "Source",
    minWidth: 170,
  },
  {
    id: "datetime",
    label: "Date Time",
    minWidth: 170,
  },
];

const useStyles = makeStyles({
  root: {
    width: "100%",
    marginTop: "80px",
  },
  container: {
    maxHeight: 440,
  },
  topRow: {
    width: "100%",
    margin: "0",
    padding: "20px",
  },
  terminateBtn: {
    position: "relative",
    textAlign: "end",
    bottom: "34px",
  },
});

const Logs = (props) => {
  const classes = useStyles();
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const { activityLogs, username } = props;
  const { getActivityLogs, terminateSession } = props;
  const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");

  useEffect(() => {
    getActivityLogs();
  }, []);

  const theme = useMemo(
    () =>
      createMuiTheme({
        palette: {
          type: prefersDarkMode ? "dark" : "light",
        },
      }),
    [prefersDarkMode]
  );

  const colorRow = theme.palette.type === "light" ? "#fafafa" : "#303030";

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(+event.target.value);
    setPage(0);
  };

  const handleTerminateSession = (evt) => {
    evt.preventDefault();
    terminateSession();
  };

  return (
    <ThemeProvider theme={theme}>
      <DrawerBar username={username} />
      <CssBaseline />
      <Paper className={classes.root}>
        <div className={classes.topRow} style={{ backgroundColor: colorRow }}>
          <Typography component="h1" variant="h5">
            Activity Logs
            <div className={classes.terminateBtn}>
              <Button
                onClick={handleTerminateSession}
                variant="contained"
                color="primary"
              >
                End Session
              </Button>
            </div>
          </Typography>
        </div>
        <TableContainer className={classes.container}>
          <Table stickyHeader aria-label="sticky table">
            <TableHead>
              <TableRow>
                {columns.map((column) => (
                  <TableCell
                    key={column.id}
                    align={column.align}
                    style={{ minWidth: column.minWidth }}
                  >
                    {column.label}
                  </TableCell>
                ))}
              </TableRow>
            </TableHead>
            <TableBody>
              {activityLogs
                .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                .map((row) => {
                  return (
                    <TableRow
                      hover
                      role="checkbox"
                      tabIndex={-1}
                      key={row.code}
                    >
                      <TableCell>{row.source}</TableCell>
                      <TableCell>{row.activity}</TableCell>
                      <TableCell>
                        {moment(Date.parse(row.time)).format(
                          "DD/MM/YYYY hh:mm A"
                        )}
                      </TableCell>
                    </TableRow>
                  );
                })}
            </TableBody>
          </Table>
        </TableContainer>
        <TablePagination
          rowsPerPageOptions={[10, 25, 100]}
          component="div"
          count={activityLogs.length}
          rowsPerPage={rowsPerPage}
          page={page}
          onChangePage={handleChangePage}
          onChangeRowsPerPage={handleChangeRowsPerPage}
        />
      </Paper>
    </ThemeProvider>
  );
};

const mapStateToProps = (state) => {
  return {
    activityLogs: state.logs.logs,
    username: state.user.userName,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    getActivityLogs: () => dispatch(getActivityLogs()),
    terminateSession: () => dispatch(terminateSession()),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(Logs);
