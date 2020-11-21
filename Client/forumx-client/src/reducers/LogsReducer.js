import { logsConstants } from '../constants/forumConstants';

const logsDefaultState = {
    logs: [],
}


export function logs(state = logsDefaultState, action ) {
    switch(action.type) {
        case logsConstants.GET_ACTIVITY_LOGS:
            return {
                logs: action.payload
            }
        default:
            return state
    };
};