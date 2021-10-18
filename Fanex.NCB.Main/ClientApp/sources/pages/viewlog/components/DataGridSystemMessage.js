import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import { DateTimeFormatV2 } from '../../constants/constants';
var dateFormat = require("dateformat");

export class DataGridSystemMessage extends Component {

    buildHeader = () => {
        return (
            <Row >
                <Cell colWidth={10} header>{Labels.MessageID}</Cell>
                <Cell colWidth={10} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={15} header>{`${Labels.CreatedDate} (GMT+8)`}</Cell>
                <Cell colWidth={30} header>{Labels.ErrorMessage}</Cell>
            </Row>
        );
    };

    buildBody = () => {
        if (this.props.systemMessages.total > 0) {
            return this.props.systemMessages.data.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="5"><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index) => (
        <Row key={index} >
            <Cell className="text-center">{item.messageID}</Cell>
            <Cell className="text-center">{item.notificationID}</Cell>
            <Cell className="text-center">{dateFormat(item.createdDate, DateTimeFormatV2)}</Cell>
            <Cell>{item.checkMsg}</Cell>
        </Row>
    );

    onPaging = (pageIndex, pageSize) => {
        const params = { notificationId : this.props.notificationId };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.getNotificationSystemMessage(params);
        this.props.getParamsPageSystemMessage(params);
    };

    render() {
        const pagingOption = {
            PageIndex: this.props.paramsPageSystemMessage.pageIndex,
            PageSize: this.props.paramsPageSystemMessage.pageSize,
            TotalItem: this.props.systemMessages.total ? this.props.systemMessages.total : 0,
            PageList: [100, 200, 300, 500, 1000]
        };

        return (
            <Table
                width={1100}
                autoWidth={false}
                header={this.buildHeader()}
                body={this.buildBody()}
                isPaging
                pageOption={pagingOption}
                onPaging={this.onPaging}
                containerPadding={0}
                adjustedHeight={50}
            />
        );
    };
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPageSystemMessage: state.paramsPageSystemMessage,
        systemMessages: state.systemMessages,
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGridSystemMessage);
