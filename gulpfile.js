const gulp = require('gulp');
const sass = require('gulp-sass')(require('sass'));

const paths = {
    scss: {
        src: 'ExpenseTracker/wwwroot/scss/**/*.scss',
        dest: 'ExpenseTracker/wwwroot/css'
    }
};

function styles() {
    return gulp.src(paths.scss.src)
        .pipe(sass().on('error', sass.logError))
        .pipe(gulp.dest(paths.scss.dest));
}

function watch() {
    gulp.watch(paths.scss.src, styles);
}

const build = gulp.series(styles, watch);

exports.styles = styles;
exports.watch = watch;
exports.default = build;