import React, { Component } from 'react';
import PropTypes from 'prop-types';

class Cell extends Component {
    static propTypes = {
        asc: PropTypes.bool,
        header: PropTypes.bool,
        onClick: PropTypes.func,
        sortable: PropTypes.bool,
        sorting: PropTypes.bool
    }

    static defaultProps = {
        header: false,
        sortable: false,
        sorting: false,
        asc: true,
        // nothing by default
        onClick: () => null
    }

    constructor(props) {
        super(props);
    }

    renderSorting(sorting, asc) {
        if (sorting) {
            return <span />;
        }

        return <span className={asc ? 'asc' : 'desc'} />;
    }

    renderHeader(rest) {
        const { sortable, sorting, asc, onClick } = this.props;
        if (sortable) {
            return (<th {...rest} className={`sortable ${sorting ? 'sorting' : ''}`} onClick={onClick}>
                {this.props.children}
                {this.renderSorting(sorting, asc)}
            </th>);
        }

        return (<th {...rest}>{this.props.children}</th>);
    }

    render() {
        const { header, colWidth, sortable, sorting, asc, onClick, ...rest } = this.props;
        return header
            ? this.renderHeader(rest)
            : <td {...rest}>{this.props.children}</td>;
    }
}

export default Cell;