import React, { Component } from 'react';
import { connect } from 'react-redux';
import { UncontrolledTooltip } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';

export class DataGrid extends Component {
  constructor(props) {
    super(props);
  }

  componentWillMount() {
    const params = {
      pageIndex: 1,
      pageSize: 10
    };

    this.props.setGridParams(params);
    this.props.getGridData(params);
  }

  onPaging = (pageIndex, pageSize) => {
    const params = {
      pageIndex: pageIndex,
      pageSize: pageSize
    };

    this.props.setGridParams(params);
    this.props.getGridData(params);
  }

  openEditPopup = (item, index) => {
    this.props.updateActiveRowIndex(index);
    this.props.openEditPopup(item);
  }

  openDeletePopup = (index, notificationTypeId, notificationTypeName) => {
    this.props.updateActiveRowIndex(index);
    this.props.openDeletePopup(notificationTypeId, notificationTypeName);
  }

  buildHeader = () => {
    return <Row style={{ height: 32 }}>
      <Cell colWidth={50} header>#</Cell>
      <Cell colWidth={200} header>Notification Type</Cell>
      <Cell colWidth={150} header>Recipient Type</Cell>
      <Cell colWidth={150} header>System</Cell>
      <Cell colWidth={150} header>Action</Cell>
    </Row>;
  }

  buildBody = (data, pageIndex, pageSize, activeRowIndex) => {
    const rows = [];
    const startIndex = (pageIndex - 1) * pageSize + 1;
    data.map((item, index) => {
      const recipientTypeNames = item.recipientTypes.map(r => r.name);
      rows.push(<Row key={`body${index}`} className={index === activeRowIndex ? 'active' : ''}>
        <Cell className="text-center">{index + startIndex}</Cell>
        <Cell>{item.notificationTypeName}</Cell>
        <Cell>{recipientTypeNames.join(', ')}</Cell>
        <Cell>{item.systemName}</Cell>
        <Cell className="text-center col-action">
          <div>
            <span className="icon-edit" id={'edit-' + index} onClick={() => this.openEditPopup(item, index)} />
            <UncontrolledTooltip placement="top" target={'edit-' + index}>Edit</UncontrolledTooltip>
            <span className="icon-delete" id={'delete-' + index} onClick={() => this.openDeletePopup(index, item.notificationTypeId, item.notificationTypeName)} />
            <UncontrolledTooltip placement="top" target={'delete-' + index}>Delete</UncontrolledTooltip>
          </div>
        </Cell>
      </Row>);
    });

    return rows;
  }

  render() {
    if (!this.props.gridData || !this.props.gridData.data) {
      return null;
    }

    const { data, count } = this.props.gridData;
    const { pageIndex, pageSize } = this.props.gridParams;
    const activeRowIndex = this.props.activeRowIndex;

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
          header={this.buildHeader()}
          body={this.buildBody(data, pageIndex, pageSize, activeRowIndex)}
          isPaging
          pageOption={pagingOption}
          onPaging={this.onPaging}
          containerPadding={0}
        />
      </div>
    );
  }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
  const { gridParams, gridData, activeRowIndex } = state;
  return {
    gridParams,
    gridData,
    activeRowIndex
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
