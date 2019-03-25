import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import { UncontrolledTooltip } from 'reactstrap';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { getMappingClass } from '../../common/CommonFunctions';
import Labels from '../../common/Labels';

class DataGrid extends Component {
    constructor(props) {
        super(props);
    }   

    onPaging = (pageIndex, pageSize) => {
        let params = { ...this.props.gridParams };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.setGridParams(params);
        this.props.getGridData(params);
    }

    openEditPopup = (item, index) => {
        this.props.filterRecipients(this.props.settingData.recipients, item.recipientTypeID);
        this.props.openEditPopup(item);
    }

    buildHeader = () => {
        return (<Row style={{ height: 32 }}>
            <Cell colWidth={30} header={true}>{Labels.NumberCol}</Cell>
            <Cell colWidth={100} header={true}>{Labels.BotName}</Cell>
            <Cell colWidth={300} header={true}>{Labels.GroupID}</Cell>
            <Cell colWidth={200} header={true}>{Labels.GroupName}</Cell>
            <Cell colWidth={100} header={true}>{Labels.RecipientType}</Cell>
            <Cell colWidth={150} header={true}>{Labels.Recipient}</Cell>
            <Cell colWidth={100} header={true}>{Labels.LicenseeStatus}</Cell>
            <Cell colWidth={100} header={true}>{Labels.MappingStatus}</Cell>
            <Cell colWidth={150} header={true}>{Labels.Remark}</Cell>
            <Cell colWidth={50} header={true}>{Labels.Action}</Cell>
        </Row>);
    }

    buildBody = (data, pageIndex, pageSize) => {
        let rows = [];
        let startIndex = (pageIndex - 1) * pageSize + 1;
        data.map((item, index) => {
            rows.push(<Row key={`body${index}`} >
                <Cell className="text-center">{index + startIndex}</Cell>
                <Cell>{item.botName}</Cell>
                <Cell>{item.conversationID}</Cell>
                <Cell>{item.groupName}</Cell>
                <Cell>{item.recipientTypeName}</Cell>
                <Cell>{item.recipientNames}</Cell>
                <Cell>{item.recipientStatusNames}</Cell>
                <Cell><span className={getMappingClass(item.mappingStatusID)}>{item.mappingStatusName}</span></Cell>
                <Cell>{item.remark}</Cell>
                <Cell className="text-center col-action">
                    <div>
                        <span className="icon-edit" id={"edit-" + index} onClick={() => this.openEditPopup(item, index)}></span>
                        <UncontrolledTooltip placement="top" target={"edit-" + index}>{Labels.Edit}</UncontrolledTooltip>                        
                    </div>
                </Cell>
            </Row>)
        })

        return rows;
    }   

    render() {
        if (!this.props.gridData || !this.props.gridData.data) {
            return null;
        }

        const data = this.props.gridData.data;
        const count = this.props.gridData.count;
        const pageIndex = this.props.gridParams.pageIndex;
        const pageSize = this.props.gridParams.pageSize;

        const pagingOption = {
            PageIndex: pageIndex,
            PageSize: pageSize,
            TotalItem: count,
            PageList: [5, 10, 20, 50, 100, 500]
        };

        return (
            <div className="table-container-div">
                <Table
                    width={1400}
                    autoWidth={false}
                    header={this.buildHeader()}
                    body={this.buildBody(data, pageIndex, pageSize)}
                    isPaging={true}
                    pageOption={pagingOption}
                    onPaging={this.onPaging}
                    containerPadding={0}
                />
            </div>
        );       
    }
}

function mapDispatchToProps(dispatch) {
    return bindActionCreators(ActionCreators, dispatch);
}

function mapStateToProps(state) {
    const { gridParams, gridData, settingData } = state;
    return Object.assign({}, { gridParams, gridData, settingData });
}

export default connect(mapStateToProps, mapDispatchToProps)(DataGrid);
