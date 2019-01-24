import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import PropTypes from 'prop-types';
import ConfirmPopup from "../../common/ConfirmPopup";

class DeletePopup extends Component {
  constructor(props) {
    super(props);    
  }

  confirmHandler = (isYes) => {
    var id = 1;
    isYes 
        ? this.props.deleteItem(id).then(this.props.closeDeletePopupHandler)
        : this.props.closeDeletePopupHandler();
  }
  
  render() {   
    if(!this.props.deletePopupData || !this.props.deletePopupData.isShowed) {
        return null;
    }

    return (
        <ConfirmPopup 
        confirmHandler={this.confirmHandler}
        confirmToggle={this.closeDeletePopupHandler}
        isShowPopup={this.props.deletePopupData.isShowed}
        title={"Delete Notification Type"}
        content={'<p>Are you sure you want to delete <br /> <strong>"' + this.props.deletePopupData.content + '"</strong></p>' }
        warningIcon={false} ></ConfirmPopup>         
    )
  }
}

DeletePopup.propTypes = {
    closeDeletePopupHandler: PropTypes.func.isRequired
};

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {deletePopupData} = state;
    return Object.assign({}, {deletePopupData});
}

export default connect(mapStateToProps, mapDispatchToProps)(DeletePopup);