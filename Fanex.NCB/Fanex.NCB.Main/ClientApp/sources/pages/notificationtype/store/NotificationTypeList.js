const receivedNotificationTypes = "RECEIVE_NOTIFICATION_TYPES";
const initState = {notificationTypes: []};

export const actionCreators = {
    getNotificationTypes : () => async(dispatch, getState) => {
        const url = '/NotificationType/GetAll';
        const response = await fetch(url, {
            method: 'GET'
        });
        const data = await response.json();

        dispatch({type: receivedNotificationTypes, data});
    }
};

export const reducer = (state, action) => {
    state = state || initState;
    if(action.type === receivedNotificationTypes){
        return {
            ...state,
            notificationTypes: action.data
        };
    }

    return state;
}