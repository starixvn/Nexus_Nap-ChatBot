import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import Labels from '../../common/Labels';
import { Table, Row, Cell } from '../../common/fixed-table-header/index';
import NoDataTitle from '../../common/no-data-title/NoDataTitle';
import { DateTimeFormatV2 } from '../../constants/constants';
var dateFormat = require("dateformat");

export class DataGridConversationMessage extends Component {

    buildHeader = () => {
        return (
            <Row >
                <Cell colWidth={15} header>{Labels.NotificationID}</Cell>
                <Cell colWidth={20} header>{Labels.BotName}</Cell>
                <Cell colWidth={20} header>{Labels.ConversationGroupID}</Cell>
                <Cell colWidth={40} header>{Labels.ConversationGroupName}</Cell>
                <Cell colWidth={20} header>{Labels.Recipient}</Cell>
                <Cell colWidth={20} header>{Labels.SentTime}</Cell>
                <Cell colWidth={15} header>{Labels.IsSuccessful}</Cell>
                <Cell colWidth={40} header>{Labels.ErrorMessage}</Cell>
                <Cell colWidth={40} header>{Labels.Content}</Cell>
            </Row>
        );
    };

    buildBody = () => {
        var conversationData = this.props.conversationMessages.data;
        var conversationDataRecipient = this.props.conversationMessages.dataMapRecipient;

        if (this.props.conversationMessages.total > 0) {
            const newData = conversationData.map((element,index) => {
                var item = conversationDataRecipient.filter((item) => item.conversationID === element.toConversationID);
                if(item.length){
                    return {...element,"recipientName": item[0].recipientName};
                }
                return element;
            });
            
            return newData.map((item, index) => this.renderRow(item, index));
        }

        return [<Row key={`body${1}`} ><Cell colSpan="10"><NoDataTitle /></Cell></Row>];
    };

    renderRow = (item, index) => (
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
    );

    onPaging = (pageIndex, pageSize) => {
        const params = { notificationId : this.props.notificationId };
        params['pageIndex'] = pageIndex;
        params['pageSize'] = pageSize;

        this.props.getConversationMessageLog(params);
        this.props.getParamsPageConversationMessage(params);
    };
    
    render() {
        const pagingOption = {
            PageIndex: this.props.paramsPageConversationMessage.pageIndex,
            PageSize: this.props.paramsPageConversationMessage.pageSize,
            TotalItem: this.props.conversationMessages.total ? this.props.conversationMessages.total : 0,
            PageList: [100, 200, 300, 500, 1000]
        };

        return (
            <Table
                width={1620}
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
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    return {
        paramsPageConversationMessage: state.paramsPageConversationMessage,
        conversationMessages: state.conversationMessages
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DataGridConversationMessage);
