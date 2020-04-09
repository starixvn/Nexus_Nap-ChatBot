import createReducer from '../../lib/createReducer';
import types from '../actions/types';

// Setting
export const msgContentTypes = createReducer({}, {
    [types.MSGCONTENTTYPES_DATA](state, action) {
        console.log(action);
        return action.data;
    }
});

// // Grid
// export const gridParams = createReducer({}, {
//     [types.GRID_PARAMS](state, action) {
//         return action.data;
//     }
// });

// export const gridData = createReducer({}, {
//     [types.GRID_DATA](state, action) {
//         return action.data;
//     }
// });

// // Delete
// export const deleteTemplatePopup = createReducer({}, {
//     [types.DELETE_POPUP_DATA](state, action) {
//         return action.data;
//     }
// });

// // Update Popup
// export const editTemplatePopup = createReducer({}, {
//     [types.EDIT_POPUP_DATA](state, action) {
//         return action.data;
//     }
// });

// export const editTemplateResult = createReducer({}, {
//     [types.EDIT_RESULT_DATA](state, action) {
//         return action.data;
//     }
// });

// // Create Popup
// export const createTemplatePopup = createReducer({}, {
//     [types.CREATE_POPUP_DATA](state, action) {
//         return action.data;
//     }
// });

// export const createTemplateResult = createReducer({}, {
//     [types.CREATE_TEMPLATE_RESULT_DATA](state, action) {
//         return action.data;
//     }
// });

// export const markdownRulesPopup = createReducer({}, {
//     [types.MARKDOWNRULE_POPUP_DATA](state, action) {
//         return action.data;
//     }
// });

const reducers = {
    msgContentTypes,
    // gridParams,
    // gridData,
    // createTemplatePopup,
    // createTemplateResult,
    // editTemplatePopup,
    // editTemplateResult,
    // deleteTemplatePopup,
    // markdownRulesPopup
};

export default reducers;