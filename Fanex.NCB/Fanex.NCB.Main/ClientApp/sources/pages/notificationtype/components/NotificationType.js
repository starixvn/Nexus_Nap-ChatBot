import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
import NCBBreadcrum from '../../common/breadcrumbs';
import ConfirmPopup from "../../common/ConfirmPopup";
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import NewNotificationType from './NewNotificationType';
import {Table, Row, Cell} from '../../common/fixed-table-header/index';

import "../../../fonts/style.css";
import './NotificationType.scss';
import "../../common/fixed-table-header/style.css";

const defaultPagingOption = {
    PageIndex: 1,
    PageSize: 10,
    TotalItem: 20,
    PageList: [10, 20, 50, 100, 500]
};

export class NotificationType extends Component {
  constructor(props) {
    super(props);
    
    this.state = {
      isShowPopup: false,
      editingItem: null,
      isShowConfirm: false,
      activeRowIndex: -1,
      deletingNotificationTypeName: ''
    };    
  }

  popupToggle = () => {
    this.setState(
      {
        isShowPopup: !this.state.isShowPopup,
        activeRowIndex: this.state.isShowPopup ? -1 : this.state.activeRowIndex
      },
      function(){
        !this.state.isShowPopup && this.props.getNotificationTypes();        
      }
    );
  }

  openPopup = (item, rowIndex) => {
    this.setState({
      editingItem: item,
      activeRowIndex: rowIndex,
      isShowPopup: true
    })
  } 

  confirmHandler = (isYes) => {
    if(isYes){
      var id = 1;
      this.props.deleteNotificationType(id).then(() => {
        this.confirmToggle();
        this.props.getNotificationTypes();
      });
    }
  }

  confirmToggle = (rowIndex, deletingNotificationTypeName) => {
    console.log("toggle");
    this.setState({
      isShowConfirm: !this.state.isShowConfirm,
      activeRowIndex: rowIndex,
      deletingNotificationTypeName: deletingNotificationTypeName
    })
  }  

  componentWillMount(){
    this.props.getNotificationTypes();
  }

  _buildHeader() {
    return (<Row style={{ height: 32 }}>
        <Cell colWidth={50} header={true}>#</Cell>
        <Cell colWidth={200} header={true}>Notification Type</Cell>
        <Cell colWidth={150} header={true}>Recipient Type</Cell>
        <Cell colWidth={150} header={true}>System</Cell>
        <Cell colWidth={150} header={true}>Action</Cell>
    </Row>);
  }

  _buildBody(data) {
    let rows = []
    data.map((item, index) => 
    {
      rows.push(<Row key={`body${index}`} className={index == this.state.activeRowIndex ? 'active' : ''}>
          <Cell className="text-center">{index + 1}</Cell>
          <Cell>{item.notificationTypeName}</Cell>
          <Cell>{item.recipientTypeName}</Cell>
          <Cell>{item.systemName}</Cell>
          <Cell className="text-center col-action">
            <div>
                <span className="icon-edit" id={"edit-" + index} onClick={() => this.openPopup(item, index)}></span>                    
                <UncontrolledTooltip placement="top" target={"edit-" + index}>Edit</UncontrolledTooltip>
                <span className="icon-delete" id={"delete-" + index} onClick={() => this.confirmToggle(index, item.notificationTypeName)}></span>
                <UncontrolledTooltip placement="top" target={"delete-" + index}>Delete</UncontrolledTooltip>
            </div>
          </Cell>
      </Row>)
    })    

    return rows;
  }

  render() {    

    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type 1"></NCBBreadcrum>
        <button className="btn-new" onClick={() => this.openPopup()}>Create New</button>          
        {
          this.props.notificationTypeListData.length &&
            <div className="table-container-div">
              <Table
                      width={900}
                      autoWidth={false}
                      header={this._buildHeader()}
                      body={this._buildBody(this.props.notificationTypeListData)}
                      isPaging={true}
                      pageOption={defaultPagingOption}
                      containerPadding={0}
                  />
            </div>
        }        
        
        {
          this.state.isShowPopup && 
            <NewNotificationType
              isShowPopup={this.state.isShowPopup}
              popupToggle={this.popupToggle}
              editingItem ={this.state.editingItem} />
        }

        {
          this.state.isShowConfirm &&
            <ConfirmPopup 
              confirmHandler={this.confirmHandler}
              confirmToggle={this.confirmToggle}
              isShowPopup={this.state.isShowConfirm}
              title={"Delete Notification Type"}
              content={'<p>Are you sure you want to delete <br /> <strong>"' + this.state.deletingNotificationTypeName + '"</strong></p>' }
              warningIcon={false} ></ConfirmPopup>            
        }
      </div>
    );
  }
}

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {notificationTypeListData, deleteNotificationTypeData} = state;
    return Object.assign({}, {notificationTypeListData, deleteNotificationTypeData});
}

export default connect(mapStateToProps, mapDispatchToProps)(NotificationType);
  