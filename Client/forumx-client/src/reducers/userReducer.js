import { userConstants } from "../constants/userConstants";

const accessToken = localStorage.getItem('access-token');
const username = localStorage.getItem('username');
const expiry = localStorage.getItem('access-expiry');

const userDefaultSate = {
    submitted: false,
    loggedIn: accessToken ? true : false,
    userName: username,
    expiry: expiry,
    errorMessage: '',
    isPasswordReset: false,
    isPasswordEdited: false,
    isTokenRefreshed: false,
    isPasswordCheckRequesting: false,
    isPasswordChecked: false,
    isPasswordCheckedException: false,
};

export function user(state=userDefaultSate, action) {
    switch(action.type) {
        case userConstants.LOGIN_SUBMITTED_SUCCESS:
            return {
                ...state,
                submitted: true,
                errorMessage: '',
            }
        case userConstants.LOGIN_SUBMITTED_FAILURE:
            return {
                ...state,
                errorMessage: action.error.message,
            }
        case userConstants.LOGIN_SUCCESS:
            return {
                ...state,
                userName: action.data.username,
                expiry: action.data.expiry,
                loggedIn: true,
                errorMessage: '',
            }
        case userConstants.LOGIN_FAILURE:
            return {
                ...state,
                loggedIn: false,
                errorMessage: action.error.message,
            }
        case userConstants.LOGOUT_SUCCESS:
            return {
                ...state,
                userName: '',
                loggedIn: false,
            }
        case userConstants.LOGOUT_FAILURE:
            return {
                otpVerified: true,
                loggedIn: true,
                errorMessage: action.error.message,
            }
        case userConstants.CLEAR_LOGIN_ERROR:
            return {
                ...state,
                submitted: false,
                errorMessage: '',
            }
        case userConstants.RESET_PASSWORD_SUCCESS:
            return {
                ...state,
                isPasswordReset: true,
                errorMessage: '',
            }
        case userConstants.RESET_PASSWORD_FAILURE:
            return {
                ...state,
                errorMessage: action.error.message,
            }
        case userConstants.EDIT_PASSWORD_SUCCESS:
            return {
                ...state,
                isPasswordEdited: true,
            }
        case userConstants.EDIT_PASSWORD_FAILURE:
            return {
                ...state,
                isPasswordEdited: false,
                errorMessage: action.error.message
            }
        case userConstants.REFRESH_TOKEN_SUCCESS:
            return {
                ...state,
                isTokenRefreshed: true,
            }
        case userConstants.REFRESH_TOKEN_FAILURE:
            return {
                ...state,
                isTokenRefreshed: false,
                errorMessage: action.payload.message
            }
        case userConstants.CHANGE_PASSWORD_CHECK_REQUESTING:
            return {
                ...state,
                isPasswordCheckRequesting: true,
            }
        case userConstants.CHANGE_PASSWORD_CHECK_SUCCESS:
            return {
                ...state,
                isPasswordCheckRequesting: false,
                isPasswordCheckedException: false,
                isPasswordChecked: action.data.status,
                errorMessage: action.data.message
            }
        case userConstants.CHANGE_PASSWORD_CHECK_FAILURE:
            return {
                ...state,
                isPasswordChecked: false,
                isPasswordCheckRequesting: false,
                isPasswordCheckedException: true,
                errorMessage: action.error.message
            }
        default:
            return state
    };
};
