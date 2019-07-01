import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import PropTypes from 'prop-types';
import ConfirmPopup from '../../common/ConfirmPopup';

export class DeletePopup extends Component {
    constructor(props) {
        super(props);
    }

    confirmHandler = isYes => {
        isYes
            ? this.props.deleteItem(this.props.deletePopupData.notificationTypeId).then(this.props.closeDeletePopupHandler)
            : this.props.closeDeletePopupHandler();
    }

    render() {
        if (!this.props.deletePopupData || !this.props.deletePopupData.isShowed) {
            return null;
        }

        return (
            <ConfirmPopup
                confirmHandler={this.confirmHandler}
                confirmToggle={this.props.closeDeletePopupHandler}
                isShowPopup={this.props.deletePopupData.isShowed}
                title={'Delete Notification Type'}
                content={`<p>Are you sure you want to delete <br /><strong>${this.props.deletePopupData.content}</strong></p>`}
                warningIcon={false}
            />
        );
    }
}

DeletePopup.propTypes = {
    closeDeletePopupHandler: PropTypes.func.isRequired
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { deletePopupData } = state;
    return {
        deletePopupData
    };
}

export default connect(mapStateToProps, mapDispatchToProps)(DeletePopup);