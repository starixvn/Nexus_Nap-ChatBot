import React, { Component } from 'react'
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import { DateTimeFormatV2 } from '../../constants/constants';
var dateFormat = require("dateformat");

export class DataGridRecipientList extends Component {

    buildHeader = isHost => {
        return (
            <Row >
                <Cell colWidth={10} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={30} header>{Labels.GroupName}</Cell>
                <Cell colWidth={10} header>{Labels.ChatGroupType}</Cell>
                <Cell colWidth={10} header>{Labels.Recipient}</Cell>
                <Cell colWidth={10} header>{Labels.SentTime}</Cell>
                <Cell colWidth={10} header>{Labels.Status}</Cell>
                <Cell colWidth={10} header>{Labels.RetryTime}</Cell>
                <Cell colWidth={30} header>{Labels.ErrorMessage}</Cell>
            </Row>);
    };

    buildBody = isHost => {
        if (this.props.recipientList.total > 0) {
            return this.props.recipientList.data.map((item, index) => this.renderRow(item, index, isHost));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="9"><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index, isHost) => (
        <Row key={index} >
            <Cell className="text-center">{item.notificationID}</Cell>
            <Cell>{item.name}</Cell>
            <Cell className="text-center">{item.chatGroupTypeName}</Cell>
            <Cell className="text-center">{item.recipientName}</Cell>
            <Cell className="text-center">{dateFormat(item.sentTime, DateTimeFormatV2)}</Cell>
            <Cell className="text-center">{item.status}</Cell>
            <Cell className="text-center">{item.retryTime}</Cell>
            <Cell>{item.errMsg}</Cell>
        </Row>
    )

    onPaging = (pageIndex, pageSize) => {
        const params = { notificationId : this.props.notificationId };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.getRecipientList(params);
        this.props.getParamsPageRecipientList(params);
    }

    render() {
        const { haveHostPermission } = this.props;
        const pagingOption = {
            PageIndex: this.props.paramsPageRecipientList.pageIndex,
            PageSize: this.props.paramsPageRecipientList.pageSize,
            TotalItem: this.props.recipientList.total ? this.props.recipientList.total : 0,
            PageList: [100, 200, 300, 500, 1000]
        };

        return (
            <Table
                width={1920}
                autoWidth={false}
                header={this.buildHeader(haveHostPermission)}
                body={this.buildBody(haveHostPermission)}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        )
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPageRecipientList: state.paramsPageRecipientList,
        recipientList: state.recipientList
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGridRecipientList)
