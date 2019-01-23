import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import {Table, Row, Cell} from '../../common/fixed-table-header/index';
import PropTypes from 'prop-types';

const propTypes = {
    onPaging: PropTypes.func.isRequired,
    getData: PropTypes.func.isRequired,
    openEditPopup: PropTypes.func.isRequired,
    openDeletePopup: PropTypes.func.isRequired,
    pageIndex: PropTypes.number.isRequired,
    pageSize: PropTypes.number.isRequired
}

export class DataGrid extends Component {
  constructor(props) {
    super(props);
    
    this.state = {
      activeRowIndex: -1      
    };    
  }

  onPaging = (pageIndex, pageSize) => {
    this.props.getData(pageIndex, pageSize); 
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
                <span className="icon-edit" id={"edit-" + index} onClick={() => this.props.openEditPopup(item, index)}></span>                    
                <UncontrolledTooltip placement="top" target={"edit-" + index}>Edit</UncontrolledTooltip>
                <span className="icon-delete" id={"delete-" + index} onClick={() => this.props.openDeletePopup(index, item.notificationTypeName)}></span>
                <UncontrolledTooltip placement="top" target={"delete-" + index}>Delete</UncontrolledTooltip>
            </div>
          </Cell>
      </Row>)
    })    

    return rows;
  }

  render() {       
    const pagingOption = {
        PageIndex: this.props.pageIndex,
        PageSize: this.props.PageSize,
        TotalItem: this.props.notificationTypeListData.data ? this.props.notificationTypeListData.count : 0,
        PageList: [10, 20, 50, 100, 500]
    };

    return (       
            this.props.notificationTypeListData.data 
            ?   <div className="table-container-div">
                    <Table
                            width={900}
                            autoWidth={false}
                            header={this._buildHeader()}
                            body={this._buildBody(this.props.notificationTypeListData.data)}
                            isPaging={true}
                            pageOption={pagingOption}
                            onPaging={this.props.onPaging}
                            containerPadding={0}
                        />
                </div>
            : null         
    );
  }
}

DataGrid.propTypes = propTypes;

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {notificationTypeListData, deleteNotificationTypeData} = state;
    return Object.assign({}, {notificationTypeListData, deleteNotificationTypeData});    
}

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
  