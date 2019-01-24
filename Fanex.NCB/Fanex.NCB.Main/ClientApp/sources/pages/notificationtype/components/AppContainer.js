import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import {NCBBreadcrum} from '../../common/breadcrumbs';
import DataGrid from './DataGrid';
import UpdatePopup from './UpdatePopup';
import DeletePopup  from './DeletePopup';

import "../../../fonts/style.css";
import './AppContainer.scss';
import "../../common/fixed-table-header/style.css";

export class AppContainer extends Component {
  constructor(props) {
    super(props);     
  }

  closeUpdatePopupHandler = () => {
    this.props.closeNewEditPopup();
    this.props.refreshUpdateData();
    this.props.updateActiveRowIndex(-1);
    !this.props.updatePopupData.isShowed && this.getGridData();
  }

  closeDeletePopupHandler = () => {
    console.log("close delete");
    this.props.closeDeletePopup();
    this.props.updateActiveRowIndex(-1);
    console.log("close delete xx");
    console.log(this.props.deletePopupData.isShowed );
    !this.props.deletePopupData.isShowed && this.getGridData();
  }
  
  componentWillMount(){
    let params = {
      pageIndex: 1, 
      pageSize: 10
    }

    this.props.setGridParams(params);
    this.props.getGridData(params.pageIndex, params.pageSize);
  }

  getGridData = () => {    
    this.props.getGridData(this.props.gridParams.pageIndex, this.props.gridParams.pageSize);
  }
  
  render() {   
    return (      
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type 1"></NCBBreadcrum>
        <button className="btn-new" onClick={this.props.openNewPopup}>Create New</button>     
        <DataGrid />               

        { this.props.updatePopupData.isShowed && <UpdatePopup closeUpdatePopupHandler={this.closeUpdatePopupHandler} /> }
        { this.props.deletePopupData.isShowed && <DeletePopup closeDeletePopupHandler={this.closeDeletePopupHandler} /> }      
      </div>
    );
  }
}

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const { gridParams, updatePopupData, deletePopupData} = state;
    return Object.assign({}, { gridParams, updatePopupData, deletePopupData});    
}

export default connect(mapStateToProps, mapDispatchToProps)(AppContainer);
  