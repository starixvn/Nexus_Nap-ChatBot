import React, { Component } from 'react';
import { connect } from 'react-redux';
import {NCBBreadcrum} from '../../common/breadcrumbs';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import NewEditPopup from './NewEditPopup';
import DataGrid from './DataGrid';
import DeletePopup  from './DeletePopup';

import "../../../fonts/style.css";
import './AppContainer.scss';
import "../../common/fixed-table-header/style.css";

export class AppContainer extends Component {
  constructor(props) {
    super(props);
     
  }

  closeNewEditPopupHandler = () => {
    this.props.closeNewEditPopup();
    this.props.refreshAddNotificationType();
    this.props.updateActiveRowIndex(-1);
    !this.props.newEditPopupData.isShowed && this.getNotificationTypes();
  }

  closeDeletePopupHandler = () => {
    console.log("close delete");
    this.props.closeDeletePopup();
    this.props.updateActiveRowIndex(-1);
    !this.props.deletePopupData.isShowed && this.getNotificationTypes();
  }
  
  componentWillMount(){
    let params = {
      pageIndex: 1, 
      pageSize: 10
    }

    this.props.setGridParams(params);
    this.props.getNotificationTypes(params.pageIndex, params.pageSize);
  }

  getNotificationTypes = () => {    
    this.props.getNotificationTypes(this.props.gridParams.pageIndex, this.props.gridParams.pageSize);
  }
  
  render() {   
    console.log("parent rerender");
    console.log(this.state);

    return (      
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type 1"></NCBBreadcrum>
        <button className="btn-new" onClick={this.props.openNewPopup}>Create New</button>     
        <DataGrid />               
        {
          this.props.newEditPopupData.isShowed && <NewEditPopup closeNewEditPopupHandler={this.closeNewEditPopupHandler} />
        }

        {
          this.props.deletePopupData.isShowed && <DeletePopup closeDeletePopupHandler={this.closeDeletePopupHandler} />
        }      
      </div>
    );
  }
}

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const { deleteNotificationTypeData, gridParams, newEditPopupData, deletePopupData} = state;
    return Object.assign({}, { deleteNotificationTypeData, gridParams, newEditPopupData, deletePopupData});    
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);
  