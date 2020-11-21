import React from 'react'
import { Redirect, Route } from 'react-router-dom'


const PrivateRoute = ({ component: Comp, isLoggedIn, path, ...rest }) => {
    return (
      <Route
        path={path}
        {...rest}
        render={props => {
          return isLoggedIn ? (
            <Comp {...props} />
          ) : (
            <Redirect
              to={{
                pathname: "/login",
                state: {
                  prevLocation: path,
                  error: "You need to login first!",
                },
              }}
            />
          );
        }}
      />
    );
  };

  export default PrivateRoute