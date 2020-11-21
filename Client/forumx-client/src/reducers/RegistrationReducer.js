import { userConstants } from '../constants/userConstants';

const registerDefaultState = {
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
}

export function register(state = registerDefaultState, action) {
    switch(action.type) {
          case userConstants.REGISTER_OAUTH_SUCCESS:
          return {
              ...state,
              isOauthSuccess: true,
              name: action.data.name,
              email: action.data.email,
            };
          case userConstants.REGISTER_OAUTH_FAILURE:
            return {
              ...state,
              errorMessage: action.error.message,
            }
          case userConstants.REGISTER_OTP_SUCCESS:
            return {
              ...state,
              otpGenerated: true,
            };
          case userConstants.REGISTER_OTP_FAILRE:
            return {
              ...state,
              otpGenerated: false,
              errorMessage: action.error.message,
            };
          case userConstants.REGISTER_SUCCESS:
            return {
              ...state,
              registerSuccess: true,
            };
          case userConstants.REGISTER_FAILURE:
            return {
              ...state,
              errorMessage: action.error.message,
            };
          case userConstants.CLEAR_REGISTER_ERROR:
            return {
              ...state,
              errorMessage: '',
            };
          case userConstants.REGISTER_USERNAME_REQUEST:
            return {
              ...state,
              requestingUsername: true
            };
          case userConstants.REGISTER_PASSWORD_REQUEST:
            return {
              ...state,
              requestingPassword: true
            };
          case userConstants.REGISTER_USERNAME_VALID:
            return {
              ...state,
              requestingUsername: false,
              usernameValid: action.data.status,
              errorMessage: action.data.message || '',
            };
            case userConstants.REGISTER_USERNAME_REQUEST_LIMIT:
              return {
                ...state,
                requestingUsername: false,
                errorMessage: action.data.message || '',
                usernameRequestLimit: true,
              };
          case userConstants.REGISTER_PASSWORD_VALID:
            return {
              ...state,
              requestingPassword: false,
              passwordValid: action.data.status,
              errorMessage: action.data.message || '',
            };
          case userConstants.REGISTER_PASSWORD_REQUEST_LIMIT:
            return {
              ...state,
              requestingPassword: false,
              errorMessage: action.data.message || '',
              passwordRequestLimit: true,
            };
          default:
            return state
    }
}