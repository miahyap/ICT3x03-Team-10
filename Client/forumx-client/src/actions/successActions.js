import { successConstants } from '../constants/forumConstants';

export const updateSuccessMessage = (message) => dispatch => {
    dispatch(success({ message: message }));
    function success(payload) { return { type: successConstants.SET_SUCCESS_MESSAGE, payload }};
}

export const clearSuccessMessage = () => dispatch => {
    dispatch(clear());
    function clear() { return { type: successConstants.CLEAR_SUCCESS_MESSAGE }};
}