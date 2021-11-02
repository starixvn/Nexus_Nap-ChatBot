import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import { DateTimeFormatV2 } from '../../constants/constants';
var dateFormat = require("dateformat");

export class DataGridRecipientList extends Component {

    buildHeader = () => {
        return (
            <Row >
                <Cell colWidth={20} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={35} header>{Labels.GroupName}</Cell>
                <Cell colWidth={15} header>{Labels.ChatGroupType}</Cell>
                <Cell colWidth={20} header>{Labels.Recipient}</Cell>
                <Cell colWidth={30} header>{`${Labels.SentTime} (GMT+8)`}</Cell>
                <Cell colWidth={15} header>{Labels.Status}</Cell>
                <Cell colWidth={15} header>{Labels.RetryTime}</Cell>
                <Cell colWidth={50} header>{Labels.ErrorMessage}</Cell>
            </Row>
        );
    };

    buildBody = () => {
        if (this.props.recipients.total > 0) {
            return this.props.recipients.data.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="9"><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index) => (
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
    );

    onPaging = (pageIndex, pageSize) => {
        const params = { notificationId : this.props.notificationId };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.getNotificationRecipientList(params);
        this.props.getParamsPageRecipientList(params);
    };

    render() {
        const pagingOption = {
            PageIndex: this.props.paramsPageRecipientList.pageIndex,
            PageSize: this.props.paramsPageRecipientList.pageSize,
            TotalItem: this.props.recipients.total ? this.props.recipients.total : 0,
            PageList: [100, 200, 300, 500, 1000]
        };

        return (
            <Table
                width={1620}
                bodyHeight={300}
                autoWidth={false}
                autoHeight={false}
                header={this.buildHeader()}
                body={this.buildBody()}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPageRecipientList: state.paramsPageRecipientList,
        recipients: state.recipients
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGridRecipientList)
