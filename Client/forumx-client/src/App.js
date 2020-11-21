import React from "react";
import { connect } from "react-redux";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import "./App.css";
import { AuthContext } from "./context/auth";
import Register from "./pages/Register";
import Login from "./pages/Login";
import Home from "./pages/Home";
import NotFound from './components/NotFound';
import PrivateRoute from '../src/helpers/PrivateRoute';
import ResetPassword from "./pages/ResetPassword";
import Profile from "./pages/Profile";
import Logs from './pages/Logs';

function App(props) {

  const { isLoggedIn } = props;

  return (
    <AuthContext.Provider value = {{ loggedIn: isLoggedIn }} >
      <div>
        <BrowserRouter>
          <Switch>
            <PrivateRoute path="/" isLoggedIn={isLoggedIn} component={Home} exact />
            <Route exact path="/login" component={Login} />
            <Route exact path="/register" component={Register} />
            <Route exact path="/resetpassword" component={ResetPassword} />
            <PrivateRoute path="/main" isLoggedIn={isLoggedIn} component={Home} exact />
            <PrivateRoute path="/main/:id" isLoggedIn={isLoggedIn} component={Home} exact/>
            <PrivateRoute path="/profile" isLoggedIn={isLoggedIn} component={Profile} exact/>
            <PrivateRoute path="/logs" isLoggedIn={isLoggedIn} component={Logs} exact/>
            <Route component={NotFound} />
          </Switch>
        </BrowserRouter>
      </div>
    </AuthContext.Provider>
  );
}

const mapStateToProps = state => {
  return {
    isLoggedIn: state.user.loggedIn,
  };
};

export default connect(mapStateToProps)(App);
