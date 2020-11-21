import { commentConstants } from '../constants/forumConstants'

const commentDefaultState = {
    isCommentCreated: false,
    isCommentUpdated: false,
    isCommentDeleted: false,
    errorMessage: '',
}

export function forumComment (state = commentDefaultState, action) {
    switch(action.type) {
        case commentConstants.CREATE_COMMENT_SUCCESS:
        return {
            ...state,
            isCommentCreated: true,
        };
        case commentConstants.CREATE_COMMENT_FAILURE:
            return {
                ...state,
                errorMessage: action.error.message
            };
        case commentConstants.UPDATE_COMMENT_SUCCESS:
            return {
                ...state,
                isCommentUpdated: true
            };
        case commentConstants.UPDATE_COMMENT_FAILURE:
            return {
                ...state,
                isCommentUpdated: false,
                errorMessage: action.error.message
            };
        case commentConstants.DELETE_COMMENT_SUCCESS:
            return {
                ...state,
                isCommentDeleted: true
            };
        case commentConstants.DELETE_COMMENT_FAILURE:
            return {
                ...state,
                errorMessage: action.error.message
            };
        case commentConstants.RESET_COMMENT_STATE:
            return {
                ...state,
                isCommentCreated: false,
                isCommentUpdated: false,
                isCommentDeleted: false,
            }
        case commentConstants.CLEAR_COMMENT_ERROR:
            return {
                ...state,
                errorMessage: '',
            }
            default:
            return state
    }
}