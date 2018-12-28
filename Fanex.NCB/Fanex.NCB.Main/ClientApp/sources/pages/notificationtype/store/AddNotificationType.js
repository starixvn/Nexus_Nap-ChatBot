const addNotificationType = "ADD_NOTIFICATION_TYPE";

export const actionCreators = {
    addNotificationType : (params) => async(dispatch, getState) => {
        const url = '/NotificationType/Add';
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify(params)
        });
        const data = await response.json();        

        dispatch({type: addNotificationType, data});
    }
};

export const reducer = (state, action) => {
    state = state || {addedresult: false};
    if(action.type === addNotificationType){
        return {
            ...state,
            addedresult: action.data
        };
    }

    return state;
}