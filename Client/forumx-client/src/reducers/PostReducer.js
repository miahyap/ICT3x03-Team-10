import { postConstants } from '../constants/forumConstants'

const postDefaultState = {
    currentPost: {},
    isPostCreated: false,
    isPostUpdated: false,
    isPostDeleted: false,
    errorMessage: '',
}

export function forumPost (state = postDefaultState, action) {
    switch(action.type) {
        case postConstants.GET_POST_SUCCESS:
            return {
                ...state,
                currentPost: action.payload,
            };
        case postConstants.GET_POST_FAILURE:
            return {
                ...state,
                currentPost: {},
                errorMessage: action.error.message,
            };
        case postConstants.CREATE_POST_SUCCESS:
            return {
                ...state,
                isPostCreated: true,
            };
        case postConstants.CREATE_POST_FAILURE:
            return {
                ...state,
                errorMessage: action.error.message
            };
        case postConstants.UPDATE_POST_SUCCESS:
            return {
                ...state,
                isPostUpdated: true,
            };
        case postConstants.UPDATE_POST_FAILURE:
            return {
                ...state,
                isPostUpdated: false,
                errorMessage: action.error.message
            };
        case postConstants.DELETE_POST_SUCCESS:
            return {
                ...state,
                isPostDeleted: true
            };
        case postConstants.DELETE_POST_FAILURE:
            return {
                ...state,
                isPostDeleted: false,
                errorMessage: action.error.message
            };
        case postConstants.RESET_POST_STATE:
            return {
                ...state,
                isPostCreated: false,
                isPostUpdated: false,
                isPostDeleted: false,
            }
        case postConstants.CLEAR_POST_ERROR:
            return {
                ...state,
                errorMessage: ''
            };
        case postConstants.CLEAR_CURRENT_POST:
            return {
                ...state,
                currentPost: {},
            }
        default:
            return state
    }
}