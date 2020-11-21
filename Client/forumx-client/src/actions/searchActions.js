import { searchConstants } from '../constants/forumConstants';
import { API } from '../helpers/config';

const SEARCH_URL = API + `Search`;

export const searchResults = (keyword, captcha) => async dispatch => {

    const token = localStorage.getItem('access-token');
    const response = await fetch(SEARCH_URL + `/${keyword}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token,
            "Captcha": captcha
        },
    });
    if (response.status !== 200) {
        dispatch(searchFailure({message: "Please ensure that you are searching for than 5 characters"}));
    }else {
        const data = await response.json();
        if (data.length === 0) {
            dispatch(searchFailure({message: 'No search results found'}));
        }else {
            dispatch(searchSuccess(data));
        }
    }
    function searchSuccess(payload) { return { type: searchConstants.SEARCH_POST_SUCCESS, payload } };
    function searchFailure(error) { return { type: searchConstants.SEARCH_POST_FAILURE, error }};
};


export const clearSearchResults = () => dispatch => {
    dispatch(clearSearch());
    function clearSearch() { return { type: searchConstants.CLEAR_SEARCH_RESULTS }};
}
