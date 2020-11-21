import { userConstants } from '../constants/userConstants';
import { updateSuccessMessage } from './successActions';
import { history } from '../helpers/history';
import { logsConstants } from '../constants/forumConstants';
import { API } from '../helpers/config';

const USER_URL = API + `Account`;

export const clearLoginError = () => dispatch => {
    dispatch(clearError());
    function clearError() { return { type: userConstants.CLEAR_LOGIN_ERROR } };
};

export const clearRegisterError = () => dispatch => {
    dispatch(clearError());
    function clearError() { return { type: userConstants.CLEAR_REGISTER_ERROR } };
};

// Needs to verify the signup endpoint
export const signUp = (userInfo) => async  dispatch => {

    const registerResponse = await fetch(USER_URL + `/RegisterAccount`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(userInfo)
    });

    if (registerResponse.status !== 200) {

        if (registerResponse.status === 400) {
            dispatch(failure({"message" : "Pleae ensure your username is unique and password is not common"}));
        }
        else if (registerResponse.status === 429) {
            dispatch(failure({"message" : "Too many requests, please try again later"}));
        }

    } else {
        localStorage.removeItem('registerOtp');
        localStorage.removeItem('registerCode');
        dispatch(success());
        history.push('/login');
    }

    function success() { return { type: userConstants.REGISTER_SUCCESS } }
    function failure(error) { return { type: userConstants.REGISTER_FAIURE, error } }
};

// Change to VerifyIdentity when commiting if not use AzureADCode
export const verifyOauth = (token) => async dispatch => {
    const response = await fetch(USER_URL + `/VerifyIdentity`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(token)
    });

    if (response.status !== 200) {
        dispatch(failure({"message": "You probably have an existing SIT Account"}));
    } else {
        const data = await response.json();
        localStorage.setItem("registerCode", data.registerCode);
        dispatch(success(data));
    }
    function success(data) { return { type: userConstants.REGISTER_OAUTH_SUCCESS, data } };
    function failure(error) { return { type: userConstants.REGISTER_OAUTH_FAILURE, error } };
}


// Verify OTP before allowing user to login
export const verifyOTP = (tokenData) =>  async dispatch => {

    const response = await fetch(USER_URL + `/VerifyOTP`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(tokenData)
    });

    if (response.status !== 200) {
        if (response.status === 400) {
            dispatch(failure({"message" : "Pleae ensure that your OTP is correct"}));
        }
        else if (response.status === 429) {
            dispatch(failure({"message" : "Too much requests. Try again later"}));
        }
    }else {
        const data = await response.json();
        let token = data.token;
        const username = data.username;
        const expiry = data.expiry;
        localStorage.setItem("access-token", token);
        localStorage.setItem("username", username);
        localStorage.setItem("access-expiry", expiry);
        dispatch(success(data));
    }
    function success(data) { return { type: userConstants.LOGIN_SUCCESS, data } }
    function failure(error) { return { type: userConstants.LOGIN_FAILURE, error } }
}

export const generateOTP = (token) => async dispatch => {
    const response = await fetch(USER_URL + `/GenerateOTP`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(token)
    });

    if (response.status !== 200) {
        dispatch(failure({"message": "Please try generate OTP again"}));
    }else if (response.status === 429) {
        dispatch(failure({"message" : "Too much requests. Try again later"}));
    } else {
        const data = await response.json();
        localStorage.setItem('registerOtp', data.message);
        dispatch(success());
    }
    function success() { return { type: userConstants.REGISTER_OTP_SUCCESS }}
    function failure(error) { return { type: userConstants.REGISTER_OTP_FAILURE, error }}

};

export const checkUsername = (usernameInfo) => async dispatch => {

    const response = await fetch(USER_URL + `/CheckUsername`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(usernameInfo)
    });

    dispatch(requesting());

    if (response.status === 429) {
        dispatch(usernameRequestLimit({message: 'Too much requests. Please refresh the page and try again.'}))
    } else if (response.status === 400) {
        dispatch(responseReceived({status: false, message: 'Something is wrong, try again later'}));
    } else {
        const data = await response.json();
        if (data.status) {
            data.message = 'Username is valid';
            dispatch(responseReceived(data));
        }else{
            data.message = 'Username has been taken';
            dispatch(responseReceived(data));
        }
    }

    function requesting() { return { type: userConstants.REGISTER_USERNAME_REQUEST }};
    function responseReceived(data) { return { type: userConstants.REGISTER_USERNAME_VALID, data }};
    function usernameRequestLimit(data) { return { type: userConstants.REGISTER_USERNAME_REQUEST_LIMIT, data }};
}


export const checkPassword = (passwordInfo) => async dispatch => {

    const response = await fetch(USER_URL + `/CheckPassword`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(passwordInfo)
    });

    dispatch(requesting());

    if (response.status === 429) {
        dispatch(passwordRequestLimit({message: 'Too much requests. Please refresh the page and try again.'}))
    } else if (response.status === 400) {
        dispatch(responseReceived({status: false, message: 'Something is wrong, try again later'}));
    } else {
        const data = await response.json();
        if (data.status) {
            data.message = 'Password is valid';
            dispatch(responseReceived(data));
        }else{
            data.message = 'Password is not a good password. Use another one';
            dispatch(responseReceived(data));
        }
    }


    function requesting() { return { type: userConstants.REGISTER_PASSWORD_REQUEST }};
    function responseReceived(data) { return { type: userConstants.REGISTER_PASSWORD_VALID, data }};
    function passwordRequestLimit(data) { return { type: userConstants.REGISTER_PASSWORD_REQUEST_LIMIT, data }};

}

//Need to verify the login endpoint
export const login = (userInfo) => async dispatch => {

    const loginResponse = await fetch(USER_URL + `/Login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(userInfo)
    });

    if (loginResponse.status !== 200) {

        let errorMessage = ""
        if (loginResponse.status === 401) {
            errorMessage = "Unathorized, please make sure your username and password is correct"
        }
        else if (loginResponse.status === 429) {
            errorMessage = "Too much requests. Try again later";
        }
        dispatch(failure({"message": errorMessage}))

    } else {
        const data = await loginResponse.json();
        localStorage.setItem("AccessToken", data.message);
        dispatch(success());
    }

    function success() { return { type: userConstants.LOGIN_SUBMITTED_SUCCESS }};
    function failure(error) { return { type: userConstants.LOGIN_SUBMITTED_FAILURE, error }};
};

export const terminateSession = () => async dispatch => {
    let token = localStorage.getItem("access-token");

    const response = await fetch(USER_URL + `/TerminateSession`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
    });

    if (response.status !== 200) {
        dispatch(failure({"message": "Unable to terminate session. Please try again"}));
    } else {
        localStorage.clear();
        dispatch(success());
        history.push('/login');
    }

    function success() { return { type: userConstants.LOGOUT_SUCCESS }};
    function failure(error) { return { type: userConstants.LOGOUT_FAILURE, error }};
};

export const resetPassword = (userInfo) => async dispatch => {
    const response = await fetch(USER_URL + `/ResetPassword`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json"
        },
        body: JSON.stringify(userInfo)
    });

    if (response.status !== 200) {
        if (response.status === 400) {
            dispatch(failure({"message" : "Please ensure that your username is correct"}));
        }
        else if (response.status === 429) {
            dispatch(failure({"message" : "Too much requests. Try again later"}));
        }else {
            dispatch(failure({"message" : "Something went wrong. Try again later"}));
        }
    } else {
        dispatch(success());
        dispatch(updateSuccessMessage("Password has been reset"));
    }
    function success() { return { type: userConstants.RESET_PASSWORD_SUCCESS } };
    function failure(error) { return { type: userConstants.RESET_PASSWORD_FAILURE, error } };
};

export const editPassword = (passwordInfo) => async dispatch => {

    let token = localStorage.getItem("access-token");
    const response = await fetch(USER_URL + `/ChangePassword`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(passwordInfo)
    });

    if (response.status !== 200) {
        if (response.status === 400) {
            dispatch(failure({"message" : "Make sure your original password is correct"}));
        }else if (response.status ===429) {
            dispatch(failure({"message": " Too many requests. Please try again later"}));
        }else {
            dispatch(failure({"message" : "Unable to update the password. Can kindly check your password"}));
        }
    }else {
        dispatch(success());
        dispatch(updateSuccessMessage("Password Succesfully updated"));
    }

    function success() { return { type: userConstants.EDIT_PASSWORD_SUCCESS } };
    function failure(error) { return { type: userConstants.EDIT_PASSWORD_FAILURE, error } };
};

export async function refreshToken(dispatch) {
    let token = localStorage.getItem("access-token");

    const response = await fetch(USER_URL + '/RefreshToken', {
        method: 'GET',
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
    });

    if (response.status !== 200) {

        dispatch({
            type: userConstants.REFRESH_TOKEN_FAILURE,
            message: 'Are you authorise to request for account?'
        });
    }else {
        const data = await response.json();
        localStorage.clear();
        localStorage.setItem("access-token", data.token);
        localStorage.setItem("username", data.username);
        localStorage.setItem("access-expiry", data.expiry);
        dispatch({
            type: userConstants.REFRESH_TOKEN_SUCCESS
        });
    }
};

// Get activity logs
export const getActivityLogs = () => async dispatch => {

    let token = localStorage.getItem("access-token");
    const response = await fetch(USER_URL + `/ActivityLogs`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        }
    });

    if (response.status === 401) {
        localStorage.clear();
        history.push('/login');
    }else {
        const data = await response.json();
        dispatch(getLogs(data));
    }
    function getLogs(payload) { return { type: logsConstants.GET_ACTIVITY_LOGS, payload } };
};

export const changePasswordCheck = (password) => async dispatch => {

    let token = localStorage.getItem("access-token");
    const response = await fetch(USER_URL + `/ChangePasswordCheck`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(password)
    });

    dispatch(requesting());

    if (response.status === 429) {
        dispatch(failure({message: 'Too much requests. Please refresh the page and try again.'}))
    } else if (response.status === 400) {
        dispatch(failure({message: 'Something is wrong, try again later'}));
    } else {
        const data = await response.json();
        if (data.status) {
            data.message = 'Password is valid';
            dispatch(success(data));
        }else{
            data.message = 'Password is not a good password';
            dispatch(success(data));
        }
    }

    function requesting() { return { type: userConstants.CHANGE_PASSWORD_CHECK_REQUESTING }};
    function success(data) { return { type: userConstants.CHANGE_PASSWORD_CHECK_SUCCESS, data } };
    function failure(error) { return { type: userConstants.CHANGE_PASSWORD_CHECK_FAILURE, error } };
}