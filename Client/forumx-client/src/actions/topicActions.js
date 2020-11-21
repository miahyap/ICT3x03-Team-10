import { topicConstants } from '../constants/forumConstants';
import { API } from '../helpers/config';
import { history } from '../helpers/history';

const TOPIC_URL = API + `Topic`;

export const getAllTopics = (bearerToken) => async dispatch => {

    const token = localStorage.getItem('access-token');
    const realToken = bearerToken || token;
    const response = await fetch(TOPIC_URL, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + realToken
        },
    });

    if (response.status !== 200 ){
        dispatch(getTopics([]));

    }else {
        const data = await response.json();
        dispatch(getTopics(data));
    }
    function getTopics(payload) { return { type: topicConstants.GET_ALL_TOPICS, payload } };

};

export const getTopicById = (topicUuid) => async dispatch => {

    const token = localStorage.getItem('access-token');
    const response = await fetch(TOPIC_URL + `/${topicUuid}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Accept": "application/json",
            "Authorization": "Bearer " + token
        },
    });

    if (response.status !== 200) {
        dispatch(getSpecificTopic({}));

        if (response.status === 401) {
            history.push('/login');
        }

    }else {
        const data = await response.json();
        dispatch(getSpecificTopic(data));
    }
    function getSpecificTopic(payload) { return { type: topicConstants.GET_TOPIC, payload }};
};

export const loadSearchResults = (searchedResult) => dispatch => {

    if (searchedResult.length > 0) {
        dispatch(loadSearchPost(searchedResult));
    }
    function loadSearchPost(payload) { return { type: topicConstants.LOAD_SEARCH_POST, payload }}
}


// this the fake getAllTopics and getTopicById : For secret use only
// const getAllTopics = () => dispatch => {

//     const allTopics = [
//         {uuid: "ad80b1ce-7e58-8b4d-99f8-8860904507e6", name: "Test 1"},
//         {uuid: "ad80b1ce-7e58-812d-99f8-8860904507e6", name: "Test 2"},
//         {uuid: "ad80b1ce-7e58-812d-99f8-8860904507e6", name: "Test 3"},
//         {uuid: "ad12b1ce-7e58-812d-99f8-8812904507e6", name: "Test 4"},
//     ]

//     dispatch(getTopics(allTopics));
//     function getTopics(payload) { return { type: topicConstants.GET_ALL_TOPICS, payload } };
// };

// const getTopicById = (uuid) => dispatch => {

//     const data = {
//         uuid: uuid,
//         name: "Test Topic",
//         posts: [
//             {uuid: "ad80b1ce-7e58-8b4d-99f8-8860904507e6", user: "test", topic: null, title: "title", content: "testcontent", edited: false, postedTime: "2020-10-27T14:58:09" comments: null }
//         ]
//     }
//     dispatch(getSpecificTopic(data));
//     function getSpecificTopic(payload) { return { type: topicConstants.GET_TOPIC, payload }};

// }