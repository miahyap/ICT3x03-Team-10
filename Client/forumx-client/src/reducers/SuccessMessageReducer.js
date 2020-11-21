import { successConstants } from '../constants/forumConstants';

const successDefaultState = {
    message: '',
    isSuccessful: false,
};

export function successMessage(state = successDefaultState, action) {

    switch(action.type) {
        case successConstants.SET_SUCCESS_MESSAGE:
            return{
                ...state,
                message: action.payload.message,
                isSuccessful: true,
            };
        case successConstants.CLEAR_SUCCESS_MESSAGE:
            return {
                ...state,
                message: '',
                isSuccessful: false,
            };
        default:
            return state
    };
};
