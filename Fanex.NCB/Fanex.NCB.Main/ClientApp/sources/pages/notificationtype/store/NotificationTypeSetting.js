const receivedSettings = "RECEIVE_SETTINGS";
const initState = {notificationTypes: []};

export const actionCreators = {
    getSettings : () => async(dispatch, getState) => {
        const url = '/NotificationType/GetSettings';
        const response = await fetch(url, {
            method: 'GET'
        });
        const data = await response.json();

        dispatch({type: receivedSettings, data});
    }
};

export const reducer = (state, action) => {
    state = state || initState;
    if(action.type === receivedSettings){
        return {
            ...state,
            settings: action.data
        };
    }

    return state;
}