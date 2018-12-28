const deletedNotificationType = "DELETED_NOTIFICATION_TYPE";

export const actionCreators = {
    deleteNotificationType : (id) => async(dispatch, getState) => {
        const url = '/NotificationType/Delete';
        const response = await fetch(url, {
            method: 'POST',
            body: JSON.stringify({id: id})
        });
        const data = await response.json();        

        dispatch({type: deletedNotificationType, data});
    }
};

export const reducer = (state, action) => {
    state = state || {deletedResult: false};
    if(action.type === deletedNotificationType){
        return {
            ...state,
            deletedResult: action.data
        };
    }

    return state;
}