import { searchConstants } from '../constants/forumConstants';

const searchDefaultState = {
    value: '',
    searched: false,
    searchedResult: [],
    message: '',
}


export function search(state = searchDefaultState, action ) {
    switch(action.type) {
        case searchConstants.SEARCH_RENDER_UPDATE:
            return {
                ...state,
                value: action.payload.value,
            }
        case searchConstants.SEARCH_POST_SUCCESS:
            return {
                ...state,
                searched: true,
                searchedResult: action.payload,
                message: 'Search Successful'
            }
        case searchConstants.SEARCH_POST_FAILURE:
            return {
                ...state,
                message: action.error.message
            }
        case searchConstants.CLEAR_SEARCH_RESULTS:
            return {
                ...state,
                searched: false,
                value: '',
                searchedResult: [],
                message: ''
            }
        default:
            return state

    };
};