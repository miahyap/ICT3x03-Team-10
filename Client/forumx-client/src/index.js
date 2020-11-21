import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import { Provider } from 'react-redux';
import { createStore, applyMiddleware, compose } from 'redux';
import thunk from 'redux-thunk';
import rootReducer from './reducers/index'
import { jwt } from './helpers/Middleware';
import { GoogleReCaptchaProvider } from 'react-google-recaptcha-v3';
import { CAPTCHA_KEY } from './helpers/config';

const store = createStore(rootReducer, compose(
  applyMiddleware(jwt, thunk),
  window.devToolsExtension ? window.devToolsExtension() : f => f
));

ReactDOM.render(
  <Provider store={store}>
    <GoogleReCaptchaProvider reCaptchaKey={CAPTCHA_KEY}>
      <App />
    </GoogleReCaptchaProvider>
  </Provider>,
  document.getElementById('root')
);
