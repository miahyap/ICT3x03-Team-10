import { postConstants } from  '../constants/forumConstants';
import { updateSuccessMessage } from './successActions';
import { API } from '../helpers/config';

const POSTURL = API + `Post`;
const token = localStorage.getItem('access-token');

export const clearPostError = () => dispatch => {
    dispatch(clearError());
    function clearError() { return { type: postConstants.CLEAR_POST_ERROR }}
};

export const resetPostState = () => dispatch => {
    dispatch(reset());
    function reset() { return { type: postConstants.RESET_POST_STATE }};
};

export const clearCurrentPost = () => dispatch => {
    dispatch(clear());
    function clear() { return { type: postConstants.CLEAR_CURRENT_POST }};
}


export const getPost = (postUuid) => async dispatch => {

    const response = await fetch(POSTURL + `/${postUuid}`, {
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token
        },
    });

    if (response.status !== 200){
        getPostFailure({"message": "Unable to get post"});
    }else{
        const data = await response.json();
        dispatch(getPostSuccess(data));
    }
    function getPostSuccess(payload) { return { type: postConstants.GET_POST_SUCCESS, payload }};
    function getPostFailure(error) { return { type: postConstants.GET_POST_FAILURE, error }};
}



export const createPost = (post) => async dispatch => {

    const response = await fetch(POSTURL + `/NewPost`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(post)
    });

    if (response.status !== 200) {
        dispatch(createPostFailure({"message" : "Please try and create post again"}));
    }else {
        dispatch(createPostSuccess());
        dispatch(updateSuccessMessage("Post succesfully created"))
    }

    function createPostSuccess() { return { type: postConstants.CREATE_POST_SUCCESS }}
    function createPostFailure(error) { return { type: postConstants.CREATE_POST_FAILURE, error }}
};

export const updatePost = (post) => async dispatch => {

    const response = await fetch(POSTURL, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(post)
    });

    if (response.status !== 200) {
        dispatch(updatePostFailure({"message" : "Please try and update again later"}));
    }else {
        dispatch(updatePostSuccess());
        dispatch(updateSuccessMessage("Post Succesfully updated"));
    }

    function updatePostSuccess() { return { type: postConstants.UPDATE_POST_SUCCESS }}
    function updatePostFailure(error) { return { type: postConstants.UPDATE_POST_FAILURE, error }}
};


export const deletePost = (postUuid) => async dispatch => {

    const response = await fetch(POSTURL + `/${postUuid}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
        body: JSON.stringify(postUuid)
    });

    if (response.status !== 200) {
        dispatch(deletePostFailure({"message" : "Unable to delete post, please try again later"}));
    }else {
        dispatch(deletePostSuccess());
        dispatch(updateSuccessMessage("Post Succesfully deleted"));
    }

    function deletePostSuccess() { return { type: postConstants.DELETE_POST_SUCCESS }}
    function deletePostFailure(error) { return { type: postConstants.DELETE_POST_FAILURE, error }}
};



// export const getPost = (postUuid) => async dispatch => {

    // const data = {
    //     uuid: null,
    //     user: 'test',
    //     topic: 'Test Topic 2',
    //     title: 'title 3',
    //     content: 'test content bla',
    //     edited: false,
    //     postedTime: '2020-10-27T14:59:54',
    //     comments: [
    //         { uuid: "5aa56e9b-7c09-4fab-b5ea-fbc7eadfce71", post: null, user: 'wtf', content: 'test-content', edited: false, postedTime: '2020-11-06T22:28:06'}
    //     ]
    // }
//        dispatch(getPostSuccess(data));
//     function getPostSuccess(payload) { return { type: postConstants.GET_POST_SUCCESS, payload }};
// }