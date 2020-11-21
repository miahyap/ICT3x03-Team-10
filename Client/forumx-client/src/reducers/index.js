import { user } from './userReducer';
import { register } from './RegistrationReducer';
import { forumTopics } from './TopicReducer';
import { forumPost } from './PostReducer';
import { forumComment } from './CommentReducer';
import { search } from './SearchReducer';
import { logs } from './LogsReducer';
import { successMessage } from './SuccessMessageReducer';

import { combineReducers } from 'redux';


const rootReducer = combineReducers({
    user,
    register,
    forumTopics,
    forumPost,
    forumComment,
    search,
    logs,
    successMessage
})

export default rootReducer;