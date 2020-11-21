import { commentConstants } from  '../constants/forumConstants';
import { updateSuccessMessage } from './successActions';
import { API } from '../helpers/config';
const token = localStorage.getItem('access-token');

const COMMENTURL = API + `Comment`;

export const clearCommentError = () => dispatch => {
    dispatch(clearError());
    function clearError() { return { type: commentConstants.CLEAR_COMMENT_ERROR }}
};

export const resetCommentState = () => dispatch => {
    dispatch(reset());
    function reset() { return { type: commentConstants.RESET_COMMENT_STATE }};
};

export const createComment = (comment) => async dispatch => {

    const response = await fetch(COMMENTURL + `/NewComment`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(comment)
    });

    if (response.status !== 200) {
        dispatch(createCommentFailure({"message" : "Please try and create comment again"}));
    }else {
        dispatch(createCommentSuccess());
        dispatch(updateSuccessMessage("Comment sucessfully created"));
    }

    function createCommentSuccess() { return { type: commentConstants.CREATE_COMMENT_SUCCESS }};
    function createCommentFailure(error) { return { type: commentConstants.CREATE_COMMENT_FAILURE, error }};
};

export const updateComment = (comment) => async dispatch => {

    const response = await fetch(COMMENTURL, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(comment)
    });

    if (response.status !== 200) {
        dispatch(updateCommentFailure({"message" : "Please update your comment later"}));
    }else {
        dispatch(updateCommentSuccess());
        dispatch(updateSuccessMessage("Comment updated"));
    }

    function updateCommentSuccess() { return { type: commentConstants.UPDATE_COMMENT_SUCCESS }}
    function updateCommentFailure(error) { return { type: commentConstants.UPDATE_COMMENT_FAILURE, error }}

};

export const deleteComment = (commentUuid) => async dispatch => {

    const response = await fetch(COMMENTURL + `/${commentUuid}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(commentUuid)
    });

    if (response.status !== 200) {
        dispatch(deleteCommentFailure({"message" : "Unable to delete comment, please try again later"}));
    }else {
        dispatch(deleteCommentSuccess());
        dispatch(updateSuccessMessage("Comment sucesfully deleted"));
    }

    function deleteCommentSuccess() { return { type: commentConstants.DELETE_COMMENT_SUCCESS }}
    function deleteCommentFailure(error) { return { type: commentConstants.DELETE_COMMENT_FAILURE, error }}

};