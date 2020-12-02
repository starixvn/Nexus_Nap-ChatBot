import React, { Component } from 'react';
import PropTypes from 'prop-types';

const propTypes = {
    pageOption: PropTypes.shape({
        PageIndex: PropTypes.number.isRequired,
        PageSize: PropTypes.number.isRequired,
        PageList: PropTypes.arrayOf(PropTypes.number).isRequired,
        TotalItem: PropTypes.number.isRequired
    }),
    onPaging: PropTypes.func.isRequired
};

export default class Pager extends Component {
    constructor(props) {
        super(props);
    }

    goToPage = (pageIndex, pageSize) => {
        this.props.onPaging(pageIndex, pageSize);
    };

    renderPageSizeSelection = (pageSize, pageList) => {
        return (
            <select className="pagesize-select" value={pageSize} onChange={e => this.goToPage(1, Number(e.target.value))}>
                {
                    pageList.map((item, index) => <option key={`pagesize${index}`} value={item}>{item}</option>)
                }
            </select>
        );
    };

    renderPageIcon = (iconClass, isDisable, onClickFunc) => {
        return (
            <a href='javascript:;'
                className={`icon ${iconClass} ${isDisable ? 'disable' : ''}`}
                onClick={() => !isDisable && onClickFunc()}
            />
        );
    };

    renderPageInfo = (index, pageSize, totalItem) => {
        const viewFrom = ((index - 1) * pageSize) + 1;
        const viewTo = index * pageSize;
        return (
            <span className="page-info">
                {`Displaying ${viewFrom} to ${viewTo > totalItem ? totalItem : viewTo} of ${totalItem} item(s)`}
            </span>
        );
    };

    calculatePages = (pageIndex, totalPage) => {
        const pages = [];
        if (totalPage < 10) {
            for (let i = 1; i <= totalPage; i++) {
                pages.push(i);
            }
        } else {
            pages.push(1);

            // left of pageIndex
            for (let i = 2; i < pageIndex; i++) {
                pageIndex - i < 4 && pages.push(i);
            }

            if (pageIndex !== 1 && pageIndex !== totalPage) {
                pages.push(pageIndex);
            }

            // right of pageIndex
            for (let j = pageIndex + 1; j < totalPage; j++) {
                j - pageIndex < 4 && pages.push(j);
            }

            pages.push(totalPage);
        }

        return pages;
    };

    renderPages = (pageIndex, pageSize, totalPage) => {
        const pages = this.calculatePages(pageIndex, totalPage);

        return (
            <div className="page-div">
                {
                    pages.map(i => <span key={i} className={'page-span ' + (i === pageIndex ? 'active' : '')} onClick={() => this.goToPage(i, pageSize)}>{i}</span>)
                }
            </div>
        );
    };

    render() {
        const { PageIndex, PageSize, TotalItem, PageList } = this.props.pageOption;
        const totalPage = Math.ceil(TotalItem / PageSize);
        const pagerWidth = this.props.width ? this.props.width + 'px' : 'unset';
        return (
            <div className="page-container" style={{ width: pagerWidth }}>
                {TotalItem > 0 && <span>Page size</span>}
                {TotalItem > 0 && this.renderPageSizeSelection(PageSize, PageList)}
                {totalPage > 1 && this.renderPageIcon('page-prev', PageIndex <= 1, () => this.goToPage(PageIndex - 1, PageSize))}
                {totalPage > 1 && this.renderPages(PageIndex, PageSize, totalPage)}
                {totalPage > 1 && this.renderPageIcon('page-next', PageIndex >= totalPage, () => this.goToPage(PageIndex + 1, PageSize))}
                {totalPage > 1 && this.renderPageInfo(PageIndex, PageSize, TotalItem)}
            </div>);
    }
}

Pager.propTypes = propTypes;