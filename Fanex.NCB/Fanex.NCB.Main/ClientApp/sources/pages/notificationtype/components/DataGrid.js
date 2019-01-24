import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
import { bindActionCreators } from 'redux';
import {ActionCreators} from '../actions/index';
import {Table, Row, Cell} from '../../common/fixed-table-header/index';

export class DataGrid extends Component {
  constructor(props) {
    super(props);
  }

  onPaging = (pageIndex, pageSize) => {
    let params = {
        pageIndex: pageIndex, 
        pageSize: pageSize
    }

    this.props.setGridParams(params); 
    this.props.getGridData(pageIndex, pageSize);     
  }

  openEditPopup = (item, index) => {
    this.props.updateActiveRowIndex(index);
    this.props.openEditPopup(item);
  }

  openDeletePopup = (index, notificationTypeName) => {
    this.props.updateActiveRowIndex(index);
    this.props.openDeletePopup(notificationTypeName) ;
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

  _buildBody(data, activeRowIndex) {
    let rows = []
    data.map((item, index) => 
    {
      rows.push(<Row key={`body${index}`} className={index == activeRowIndex ? 'active' : ''}>
          <Cell className="text-center">{index + 1}</Cell>
          <Cell>{item.notificationTypeName}</Cell>
          <Cell>{item.recipientTypeName}</Cell>
          <Cell>{item.systemName}</Cell>
          <Cell className="text-center col-action">
            <div>
                <span className="icon-edit" id={"edit-" + index} onClick={() => this.openEditPopup(item, index)}></span>                    
                <UncontrolledTooltip placement="top" target={"edit-" + index}>Edit</UncontrolledTooltip>
                <span className="icon-delete" id={"delete-" + index} onClick={() => this.openDeletePopup(index, item.notificationTypeName)}></span>
                <UncontrolledTooltip placement="top" target={"delete-" + index}>Delete</UncontrolledTooltip>
            </div>
          </Cell>
      </Row>)
    })    

    return rows;
  }

  render() {      
    if(!this.props.gridData || !this.props.gridData.data){
        return null;
    }

    const data = this.props.gridData.data;
    const activeRowIndex= this.props.activeRowIndex;
    const count = this.props.gridData.count;
    const pageIndex = this.props.gridParams.pageIndex;
    const pageSize = this.props.gridParams.pageSize;

    const pagingOption = {
        PageIndex: pageIndex,
        PageSize: pageSize,
        TotalItem: count,
        PageList: [10, 20, 50, 100, 500]
    };

    return (       
        <div className="table-container-div">
            <Table
                    width={900}
                    autoWidth={false}
                    header={this._buildHeader()}
                    body={this._buildBody(data, activeRowIndex)}
                    isPaging={true}
                    pageOption={pagingOption}
                    onPaging={this.onPaging}
                    containerPadding={0}
                />
        </div>     
    );
  }
}

function mapDispatchToProps(dispatch){
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state){
    const {gridParams, gridData, activeRowIndex} = state;
    return Object.assign({}, {gridParams, gridData, activeRowIndex});    
}

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
  