import { topicConstants } from '../constants/forumConstants'

const topicDefaultState = {
    topics: [],
    currentTopic: {},
}

export function forumTopics (state = topicDefaultState, action){

    switch(action.type) {
        case topicConstants.GET_ALL_TOPICS:
        return {
            ...state,
            topics: action.payload,
        };
        case topicConstants.GET_TOPIC:
            return {
                ...state,
                currentTopic: action.payload
            };
        case topicConstants.LOAD_SEARCH_POST:
            return {
                ...state,
                currentTopic: {
                    posts: action.payload
                }
            }
        default:
            return state
    }


}
