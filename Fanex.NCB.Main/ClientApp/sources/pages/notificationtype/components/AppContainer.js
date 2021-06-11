import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { NCBBreadcrum } from '../../common/breadcrumbs/breadcrumbs';
import DataGrid from './DataGrid';
import CreateEditPopup from './CreateEditPopup';
import DeletePopup from './DeletePopup';

import '../../shared/components/font.css';
import './AppContainer.scss';

export class AppContainer extends Component {
    static defaultProps = {
        isHost: false,
        gridParams: {},
        createEditPopupData: { isShowed: false },
        deletePopupData: { isShowed: false }
    };

    handleOpenCreatePopup = () => {
        this.props.setCreateEditPopupData({
            isShowed: true,
            editingItem: {
                notificationTypeName: {
                    value: '',
                    isValid: false
                },
                recipientTypeIds: {
                    value: '',
                    isValid: false
                },
                chatGroupTypeIds: {
                    value: '',
                    isValid: false
                },
                systemId: {
                    value: 0,
                    isValid: false
                }
            },
            popupTitle: 'Create New Notification Type',
            successMsg: 'Create New Notification Type Successfully',
            isNew: true
        });
    }

    handleCloseCreateEditPopup = () => {
        this.props.setCreateEditPopupData({ isShowed: false });
        this.props.setCreateEditResult(null);
        this.props.setActiveRowIndex(-1);
        !this.props.createEditPopupData.isShowed && this.props.getGridData(this.props.gridParams);
    }

    handleCloseDeletePopup = () => {
        this.props.setDeletePopupData({ isShowed: false });
        this.props.setDeleteResult(null);
        this.props.setActiveRowIndex(-1);
        !this.props.deletePopupData.isShowed && this.props.getGridData(this.props.gridParams);
    }

    render() {
        return (
            <div>
                <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type" />
                {
                    this.props.isHost
                    && <button className="btn-new" onClick={this.handleOpenCreatePopup}>New Notification Type</button>
                }
                <DataGrid isHost={this.props.isHost} />
                {
                    this.props.createEditPopupData.isShowed
                    && <CreateEditPopup onClose={this.handleCloseCreateEditPopup} />
                }
                {
                    this.props.deletePopupData.isShowed
                    && <DeletePopup onClose={this.handleCloseDeletePopup} />
                }
            </div>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { gridParams, createEditPopupData, deletePopupData } = state;
    return {
        gridParams,
        createEditPopupData,
        deletePopupData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);