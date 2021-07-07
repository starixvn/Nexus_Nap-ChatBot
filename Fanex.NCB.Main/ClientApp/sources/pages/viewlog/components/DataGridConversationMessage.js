import React, { Component } from 'react'
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import { DateTimeFormatV2 } from '../../constants/constants';
var dateFormat = require("dateformat");

export class DataGridConversationMessage extends Component {

    buildHeader = isHost => {
        return (
            <Row >
                <Cell colWidth={20} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={20} header>{Labels.BotName}</Cell>
                <Cell colWidth={22} header>{Labels.ConversationGroupID}</Cell>
                <Cell colWidth={40} header>{Labels.ConversationGroupName}</Cell>
                <Cell colWidth={20} header>{Labels.Recipient}</Cell>
                <Cell colWidth={25} header>{Labels.SentTime}</Cell>
                <Cell colWidth={20} header>{Labels.IsSuccessful}</Cell>
                <Cell colWidth={40} header>{Labels.ErrorMessage}</Cell>
                <Cell colWidth={40} header>{Labels.Content}</Cell>
            </Row>);
    };

    buildBody = isHost => {
        if (this.props.conversationMessage.data) {
            return this.props.conversationMessage.data.map((item, index) => this.renderRow(item, index, isHost));
        }
        return [<Row key={`body${1}`} ><Cell colSpan="5"><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index, isHost) => (
        <Row key={index} >
            <Cell className="text-center">{item.notificationID}</Cell>
            <Cell className="text-center">{item.botName}</Cell>
            <Cell className="text-center">{item.conversationGroupID}</Cell>
            <Cell>{item.conversationGroupName}</Cell>
            <Cell className="text-center">{item.recipientName}</Cell>
            <Cell className="text-center">{dateFormat(item.sentTime, DateTimeFormatV2)}</Cell>
            <Cell className="text-center">{item.isSuccessful}</Cell>
            <Cell>{item.errorMessage}</Cell>
            <Cell>{item.content}</Cell>
        </Row>
    )

    onPaging = (pageIndex, pageSize) => {
        const params = { notificationId : this.props.notificationId };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.getConversationMessage(params);
        this.props.getParamsPage(params);
    }
    
    render() {
        const { haveHostPermission } = this.props;
        const pagingOption = {
            PageIndex: this.props.paramsPage.pageIndex,
            PageSize: this.props.paramsPage.pageSize,
            TotalItem: this.props.conversationMessage.total ? this.props.conversationMessage.total : 1,
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
        paramsPage: state.paramsPage,
        conversationMessage: state.conversationMessage
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGridConversationMessage)
