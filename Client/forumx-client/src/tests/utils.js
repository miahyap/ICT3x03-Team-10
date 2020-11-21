// test-utils.js
import React from 'react'
import { render as rtlRender } from '@testing-library/react'
import { createStore } from 'redux'
import { Provider } from 'react-redux'
// Import your own reducer
import rootReducer from '../reducers/index';

function render(
  ui,
  {
    initialState,
    store = createStore(rootReducer, initialState),
    ...renderOptions
  } = {}
) {
  function Wrapper({ children }) {
    return <Provider store={store}>{children}</Provider>
  }
  return rtlRender(ui, { wrapper: Wrapper, ...renderOptions })
}

// re-export everything
export * from '@testing-library/react'
// override render method
export { render }

export const initialState = {
    user: {
        submitted: false,
        loggedIn: false,
        userName: false,
        expiry: false,
        errorMessage: '',
        isPasswordReset: false,
        isPasswordEdited: false,
        isTokenRefreshed: false,
        isPasswordCheckRequesting: false,
        isPasswordChecked: false,
        isPasswordCheckedException: false,
    },
    register: {
        isOauthSuccess: false,
        name: '',
        email: '',
        otpGenerated: false,
        registerSuccess: false,
        errorMessage: '',
        requestingUsername: false,
        usernameValid: false,
        requestingPassword: false,
        passwordValid: false,
        usernameRequestLimit: false,
        passwordRequestLimit: false,
    },
    forumTopics: {
        topics: [],
        currentTopic: {},
    },
    forumPost: {
        currentPost: {},
        isPostCreated: false,
        isPostUpdated: false,
        isPostDeleted: false,
        errorMessage: '',
    },
    logs: {
        logs: [],
    },
    search: {
        value: '',
        searched: false,
        searchedResult: [],
        message: '',
    },
    successMessage : {
        message: '',
        isSuccessful: false,
    },
    forumComment: {
        isCommentCreated: false,
        isCommentUpdated: false,
        isCommentDeleted: false,
        errorMessage: '',
    }
};

export const mockResponse = (status, response) => {
    return new window.Response(response, {
        status: status,
        headers: {
            "Content-Type": "application/json"
        },
    });
};
